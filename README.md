# README #
<img width="324" height="158" alt="Light mode (1)" src="https://github.com/user-attachments/assets/185a1790-7487-408f-bec7-5ebf311dcdd1" />

This is a prototype of **Foenn**, a geospatial analytics platform.

# Objective

The goal of this prototype is to work on a project I had in mind for a while, challenge myself with new technical topics, and document findings and outcomes.

This first step is a **proof of concept** for a end-to-end pipeline, from ingesting raw geospatial data to displaying aggregated data on a map, while also exploring a few potential customer use cases.

Building a first all-in-one software package was a deliberate choice to minimize the time required to complete a small usable prototype, focusing on technical discovery and intentionally setting aside any larger-scale implementation concerns.

Publishing the codebase is also a way to set a clear milestone for the project, helping me focus on a first iteration and write down ideas for the next steps.

# Scope

1. Build an **ETL** pipeline that loads weather history files (in CSV) into a database (SQLite for the prototype) using an analytics-ready **Star Schema**.  
   File download automation and delta loading were intentionally excluded, considering all files used in this prototype were already complete and immutable.

2. Create a **Query Execution Engine** that produces geolocated results.
   [SqlKata](https://sqlkata.com/) was reused for SQL generation.

4. Display results on an **Interactive Map**, using the [OpenStreetMap API](https://www.openstreetmap.org/) to download tiles and generate **Heatmaps** with a CPU-based algorithm.  
   Geolocated measures are displayed with Unity through prefabs generation and camera movement handlers.

5. Explore a few customer use cases to see how higher-level models could be built on top of a geo engine, such as:
   1. A **Weather Day Report** for a specified location
   2. A **Temporal Heatmap** for a location, to visualize the overall climate over a year
   3. A generic **Statistics Overview** of weather history, with heatmaps and aggregation locations, including a few aggregations, metrics, and an example using dry hours
   4. An **Outdoor Activity Selection** feature, indicating the number of hours during which an activity can be performed. The activity is defined as a wrapper around customizable weather conditions.
   5. An **Activity Planner**, to determine the best activity for a location and when to do it

# Technical Deep Dive

## Development Platform

I decided to take Unity as a development platform. My goal was to move quickly, focus on product and technical discovery, and produce concrete outcomes rather than invest in long-term code reusability. Given my experience int building interactive applications in C# with Unity, and because the prototype required background processing, rendering, and navigation, Unity was the most efficient way to validate the core ideas under those constraints.
<img width="1364" height="589" alt="Capture d&#39;écran 2026-04-10 100945" src="https://github.com/user-attachments/assets/12f25e7e-4b18-4833-8ba4-dcdb7e778a95" />

## Architecture
 - **Application Flow Diagram**
 <img width="3282" height="812" alt="Foenn POCArchitecture drawio" src="https://github.com/user-attachments/assets/866c811e-52d9-4810-85b7-ae54a013b1a8" />

- **Dataset Class Diagram**
  <img width="1882" height="522" alt="Foenn POCDataset drawio" src="https://github.com/user-attachments/assets/e621cbd1-ba8f-4cad-a0f0-13eea5b19fb1" />

- **Dataset definition**
  - The prototype includes one dataset: **Weather History Hourly Report** with a pre-aggregation on year level to handle some BI-oriented queries.
    https://github.com/jorismaillet/foenn-atlas-prototype/blob/2689cde933427c6a1e0db5605bed818797f9d1ba/Assets/Scripts/OLAP/Datasets/WeatherHistory/WeatherHistoryDataset.cs#L12-L22
    https://github.com/jorismaillet/foenn-atlas-prototype/blob/2689cde933427c6a1e0db5605bed818797f9d1ba/Assets/Scripts/OLAP/Datasets/WeatherHistory/Dimensions/TimeDimension.cs#L9-L47
    https://github.com/jorismaillet/foenn-atlas-prototype/blob/2689cde933427c6a1e0db5605bed818797f9d1ba/Assets/Scripts/OLAP/Datasets/WeatherHistory/Dimensions/LocationDimension.cs#L8-L40
    https://github.com/jorismaillet/foenn-atlas-prototype/blob/5edf6ba9442adc2f0718b920eb3609f646255073/Assets/Scripts/OLAP/Datasets/WeatherHistory/Facts/WeatherCoreFact.cs#L9-L41
     https://github.com/jorismaillet/foenn-atlas-prototype/blob/5edf6ba9442adc2f0718b920eb3609f646255073/Assets/Scripts/OLAP/Datasets/WeatherHistory/Facts/WeatherWindFact.cs#L12-L19
    https://github.com/jorismaillet/foenn-atlas-prototype/blob/2689cde933427c6a1e0db5605bed818797f9d1ba/Assets/Scripts/OLAP/Datasets/WeatherHistory/Facts/WeatherYearlyFact.cs#L13-L20

- **ETL**
  - Extraction extracts CSV column headers to precompute value lookup, and stream each line into a buffer for transformation
    https://github.com/jorismaillet/foenn-atlas-prototype/blob/c1bc9e4ad2d67ff55956eb37c98e7d2ee40dc6d7/Assets/Scripts/ETL/Extractors/CSVExtractor.cs#L26-L37
  - Transformation is handled by each field definition through field mapping. Several cases had to be handled:
     - A simple field load
     https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/OLAP/Datasets/WeatherHistory/Facts/WeatherCoreFact.cs#L39
     - A field leading to multiple attributes with different transformations
     https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/OLAP/Datasets/WeatherHistory/Dimensions/TimeDimension.cs#L40-L46
     - Multiple fields into one metric, as posts can deliberately chose their metric key
     https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/OLAP/Datasets/WeatherHistory/Facts/WeatherCoreFact.cs#L28-L34
  - Loading is structured around similar actions:
    - **Staging**: insert batches of rows into a temporary table with staging pragmas and without constraint validation
    - **Merge**: ask the DBMS to move staged data into the final table while performing validation and building indexes
    - **Derivation**: transform merged data into another table through aggregations
    - **Caching**: keep track of inserted dimension IDs through lookup fields, used later for fact insertion and derivation
   - The ETL Processor executes the operation sequence:
https://github.com/jorismaillet/foenn-atlas-prototype/blob/5e54ec9b95d1dcafc537de8c3a481852644277aa/Assets/Scripts/ETL/ETLProcessor.cs#L44-L52

- **Query Engine**
  - A [Query Request](https://github.com/jorismaillet/foenn-atlas-prototype/blob/main/Assets/Scripts/OLAP/Engine/QueryRequest.cs) is built with SQLKata **Query Builder**, accepts field elements and returns a [Query Result](https://github.com/jorismaillet/foenn-atlas-prototype/blob/main/Assets/Scripts/OLAP/Engine/QueryResult.cs).
  - `QueryResult` parses DB results and exposes geolocated rows with convenient field value access.
  https://github.com/jorismaillet/foenn-atlas-prototype/blob/5e54ec9b95d1dcafc537de8c3a481852644277aa/Assets/Scripts/OLAP/Engine/QueryResult.cs#L55-L61
  https://github.com/jorismaillet/foenn-atlas-prototype/blob/5e54ec9b95d1dcafc537de8c3a481852644277aa/Assets/Scripts/OLAP/Engine/Row.cs#L10-L12
- **Map Rendering**
  - For each geolocated result, the selected measure is collected and positioned on the world map through a tile-to-UI converter.
  https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/Helpers/GeoHelper.cs#L37-L47

- **Heatmap Generation**
  - [Compressed Sparsed Row](https://en.wikipedia.org/wiki/Sparse_matrix) is used for spatial indexing and [Inverse Distance Weighting](https://en.wikipedia.org/wiki/Inverse_distance_weighting) for spatial interpolation. This CPU-based algorithm computes the data on a low-resolution grid, then upscales it into a map layer with custom color gradent, based on the displayed metric field. 
  https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/Interface/Visualisations/Heatmap/HeatmapGenerator.cs#L55-L61
   - The color gradient can be fixed, like temperature (blue = cold, red = hot), or relative, and depends on a query result (ex: displaying the number of hours available of an activity, red = 0, green = maximum observation value)

- **UI Orchestration**
  - Each scenario has its own component, requires different input fields, and relies on the `WeatherQueryService` for execution.
  https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/Interface/Components/Views/Cases/ActivityStatistics.cs#L20-L25
  - The background map layer is generated through the **OpenStreetMap API** and cached locally.
  https://github.com/jorismaillet/foenn-atlas-prototype/blob/8219e0a8717d04b9ffe4269730cd1d807eba180e/Assets/Scripts/Interface/Components/Layers/OpenStreetMapGridRenderer.cs#L67-L74

# Limitations

- This is a prototype, so I deliberately excluded important reliability and production-readiness standards in order to focus on research and discovery. As a result, light test coverage, bugs, dead code, and rough edges are expected.
- The technologies used, compared to the amount of data processed, make this prototype unpleasant to use in practice. This was intentional: I wanted to push the basic foundations to their limits in order to clearly identify future improvement paths.

# Usage

This prototype is not intended to be reused as-is. The following steps simply describe how I used it and how the prototype works.

1. Download Open Data files for **Hourly Weather Records**:  
   https://meteo.data.gouv.fr/datasets/6569b4473bedf2e7abad3b72
2. Set basic seed data in a seed file
3. Run the application and wait for files to be loaded into the database
4. Explore the scenarios

The interface is organized as follows:

1. Scenarios are selected from a collapsible left-side menu
2. Scenario parameters are configured in a top banner with a **Go** button
3. Depending on the scenario, the main view displays either fixed information, scrollable content, or a navigable map with drag-and-drop and mouse wheel controls

# Results
- Heatmap (2020)
   - Temperature: Minimum, Average, Maximum
      <img width="1516" height="849" alt="image" src="https://github.com/user-attachments/assets/6b114b2b-08cb-45c2-ba1e-a290b57881a1" />
      <img width="1516" height="853" alt="image" src="https://github.com/user-attachments/assets/993d54d1-acd8-4d27-8451-45bac79ed2fe" />
      <img width="1516" height="850" alt="image" src="https://github.com/user-attachments/assets/be6fb473-d468-46f1-9559-37136580f44f" />
   - Rain: Maximum
     <img width="1508" height="846" alt="image" src="https://github.com/user-attachments/assets/06258f69-78d4-4e60-af2b-af88bca92e1f" />
   - Wind: Maximum
     <img width="1515" height="849" alt="image" src="https://github.com/user-attachments/assets/2aab8ed2-67fc-4d2a-b817-a3df71d20384" />
- Temporal Heatmap (2024)
  - Montpellier
    <img width="1231" height="693" alt="image" src="https://github.com/user-attachments/assets/934ee424-b85e-4d7f-9c8a-2be19e5351b2" />
  - Quimper
    <img width="1230" height="690" alt="image" src="https://github.com/user-attachments/assets/092c8a96-251c-4ceb-b351-c9a050d85ca5" />
  - Paris
    <img width="1230" height="693" alt="image" src="https://github.com/user-attachments/assets/cd9e70f9-421d-4771-a312-0dc3f3580116" />
- Activity Heatmap. This is an example of very arbitrary conditions, but they were selected to validate the querying layer:
  - Year: 2024, Temperature: Between 6 and 25 Celsius Degrees, Time: Between 9am and 7pm, Rain: Below 1mm per hour, Wind speed: Below 2 m/s 
```SQL
SELECT "location_dimension"."id", "location_dimension"."lon", "location_dimension"."lat", "location_dimension"."post_name", COUNT("weather_history_facts"."id")
FROM "weather_history_facts" 
INNER JOIN "location_dimension" ON "weather_history_facts"."location_id" = "location_dimension"."id"
INNER JOIN "time_dimension" ON "weather_history_facts"."time_id" = "time_dimension"."id"
WHERE "time_dimension"."year" = 2024 AND "weather_history_facts"."temperature" BETWEEN 6 AND 25 AND "time_dimension"."hour" BETWEEN 9 AND 19 AND "weather_history_facts"."rain" BETWEEN 0 AND 1 AND "weather_history_facts"."wind_speed" BETWEEN 0 AND 2
GROUP BY "location_dimension"."id", "location_dimension"."lon", "location_dimension"."lat", "location_dimension"."post_name";
```
  <img width="1557" height="874" alt="image" src="https://github.com/user-attachments/assets/979807c2-2652-4588-8761-18619eb8b9d7" />

# Evolution Paths (#MVP)

## Stack

- Python
- ReactJS
- PostgreSQL
- PostGIS

## Performance

### ETL

- Reduce ETL time (load raw files directly in a staging table with DuckDB for example, or load Parquet instead)
- Pre-aggregate more facts depending on time levels and location levels.  

### Query Engine

- Delegate spatial querying to PostGIS
- Add sampling based on the displayed area
- Chose dyanmical the data depending on level of zoom
- Chose dynamically the aggregated fact table depending on the time aggregation and navigation

### Rendering

- GPU rendering for layers such as heatmaps
- Derive aggregates for each time level (day, month, year) and location dimension (department)
- Move more transformations to the database layer
- Apply zoom and pan culling for display

## Features

- Geospatial interpolation for any request instead of relying only on fixed observation locations
- Better customer quality of life, customization, and product features
- Common OLAP operations, like decomposition, drill-down, etc.
  
### Data Sources

- Multiple datasets (historical + forecast)
- Customer datasets and data blending

## Automation

- Downloader service
- Delta jobs
- Cron jobs for derived tables (hour, day, month, year)

## Production readiness
Scalability, multi-tenancy, reliability, sercurity... All standards to provide a real Saas product.
