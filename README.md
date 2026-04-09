# README #

This is a prototype of **Foenn**, a geospatial analytics platform.

# Objective

The goal of this prototype is to work on a project I had in mind for a while, challenge myself with new technical topics, and document findings and outcomes.

Publishing the codebase is also a way to set a clear milestone for the project, helping me focus on a first iteration and write down ideas for the next steps.

This first step is a **proof of concept** for a end-to-end pipeline, from ingesting raw geospatial data to displaying aggregated data on a map, while also exploring a few potential customer use cases.

Building a first all-in-one software package was a deliberate choice to minimize the time required to complete a small usable prototype, focusing on technical discovery and intentionally setting aside any larger-scale implementation concerns.

# Scope

1. Build an **ETL** pipeline that loads weather history files (in CSV) into a database (SQLite for the prototype) using an analytics-ready **Star Schema**.  
   File download automation and delta loading were intentionally excluded, considering all files used in this prototype were already complete and immutable.

2. Create a **Query Execution Engine** that produces geolocated results.
3. **SqlKata** was reused for SQL generation.

4. Display results on an **Interactive Map**, using the **OpenStreetMap API** to download tiles and generate **Heatmaps** with a CPU-based algorithm.  
   Geolocated measures are displayed through Unity through prefabs generation and camera movement handlers.

5. Explore a few customer use cases to see how higher-level models could be built on top of a geo engine, such as:
   1. A **Weather Day Report** for a specified location
   2. A **Temporal Heatmap** for a location, to visualize the overall climate over a year
   3. A generic **Statistics Overview** of weather history, with heatmaps and aggregation locations, including a few aggregations, metrics, and an example using dry hours
   4. An **Outdoor Activity Selection** feature, indicating the number of hours during which an activity can be performed. The activity is defined as a wrapper around customizable weather conditions.
   5. An **Activity Planner**, to determine the best activity for a location and when to do it

# Technical Deep Dive

## Development Platform

Unity was, for me, the best choice for this prototype. I am very familiar with C# development through Unity, as I have always built small games with it. Unity provides a simple interface builder and useful features for background processing, texture rendering, and map navigation. C# is quite academic, but helpful when iterating on a full-stack discovery and implementation process.

This was also a strategic choice: it creates a clear separation between a safe-to-publish prototype for a portfolio and a potential real SaaS product.

## Architecture

- **Schema**
  - Define key metadata for tables (name, primary key, indexes), fields (display name, DB type, analytics type), and field mappings (source file column, optional transformation)
  - Add specific metadata for dimensions (lookup field) and fact tables (list of dimensions)

- **Datasets**
  - Represent a complete analytics concept. In this case, the prototype includes a **Weather History** dataset, containing:
    - A time dimension (year, hour, day, etc.)
    - A location dimension (observation posts)
    - Fact tables:
      - one fact table per observation category (wind, temperature, etc.)
      - one core fact table centralizing the most commonly used metrics
      - one pre-aggregated fact table on time dimension for generic BI-oriented queries

- **ETL**
  - **CSV Extractor**
    - Extract column headers to precompute value lookup
    - Stream each line into a buffer for transformation
  - Transformation is handled by each field definition through field mapping.
  - Loading is structured around similar actions:
    - **Staging**: insert batches of rows into a temporary table without constraint validation
    - **Merge**: ask the DBMS to move staged data into the final table while performing validation and building indexes
    - **Derivation**: transform merged data into another table through aggregations
    - **Caching**: keep track of inserted dimension IDs through lookup fields, used later for fact insertion and derivation

  The loading process is then run in two phases:
  - A first CSV read to **stage**, **merge**, and **cache** dimensions
  - A second CSV read to **stage**, **merge**, and **derive** facts

- **Query Engine**
  - A query request is built through a **Query Builder** object used for analytics requests, with **SqlKata** generating the SQL.
  - It accepts `Field` elements and returns a `QueryResult` after execution against a SQLite connection.
  - `QueryResult` parses DB results and exposes geolocated rows with convenient field value access.

- **Map Rendering**
  - For each geolocated result, the selected measure is collected and positioned on the world map through a tile-to-UI converter.

- **Heatmap Generation**
  - **CSR** is used for spatial indexing and **Inverse Distance Weighting (IDW)** for spatial interpolation.
  - This CPU-based algorithm computes the metric on a low-resolution grid, then upscales it into a map layer with custom color grading based on the displayed metric.

- **UI Orchestration**
  - Each scenario has its own component, requires different input fields, and relies on the `WeatherQueryService` for execution.
  - The background map layer is generated through the **OpenStreetMap API** and cached locally.

## Key Mechanics

- **CSV extraction**  
  `500 MB` files are read through a stream reader to reduce memory usage, with a string buffer to reduce allocations.  
  Future improvements: multithreading, delegating loading to more performant tools (for example DuckDB), and moving from in-memory transformations to SQL-based transformations.

- **Facts requiring dimension IDs**  
  Use a dimension cache and lookup fields, and load dimensions before facts.

- **Loading**  
  Batch loading with staging tables and SQLite pragmas.

- **Low-performance analytics**  
  Pre-aggregate facts at different time levels.  
  The first test used yearly derivation. Customer requests can then be redirected to pre-aggregated tables.

- **Multiple definitions of types**  
  Centralize type definitions into field types (source, map behavior, analytics usage, color gradient, display format).

- **Heavy heatmap computation**  
  Optimize IDW. Later options include PostGIS or GPU shaders.

- **Map display**  
  Use a hardcoded zoom level to keep relying on cached data.  
  Later: a more robust OpenStreetMap integration.

- **Geolocation display**  
  Apply zoom-based culling.

- **Common OLAP methods**  
  Start with member retrieval. Future work could include decomposition, drill-down, and related OLAP operations.

# Limitations

- This is a prototype, so I deliberately excluded important reliability and production-readiness standards in order to focus on research and discovery. As a result, light test coverage, bugs, dead code, and rough edges are expected.
- The technologies used, compared to the amount of data processed, make this prototype unpleasant to use in practice. This was intentional: I wanted to push the basic foundations to their limits in order to clearly identify future improvement paths.

# Usage

This prototype is not intended to be reused as-is. The following steps simply describe how I used it and how the prototype works.

1. Download Open Data files for **Hourly Weather Records**:  
   https://meteo.data.gouv.fr/datasets/6569b4473bedf2e7abad3b72
2. Run the application once to load the files into the database
3. Set basic seed data in a seed file
4. Run the application and explore the scenarios

The interface is organized as follows:

1. Scenarios are selected from a collapsible left-side menu
2. Scenario parameters are configured in a top banner with a **Go** button
3. Depending on the scenario, the main view displays either fixed information, scrollable content, or a navigable map with drag-and-drop and mouse wheel controls

# Results


# Evolution Paths (MVP)

## Stack

- Python
- ReactJS
- PostgreSQL
- PostGIS

## Performance

### ETL

- Reduce ETL time (load raw files as-is, or load Parquet instead)

### Query Engine

- Delegate spatial querying to PostGIS
- Add sampling based on the displayed area

### Rendering

- GPU rendering for layers such as heatmaps
- Derive aggregates for each time level (day, month, year) and location dimension (department)
- Move more transformations to the database layer
- Apply zoom and pan culling for display
- Apply zoom and pan sampling for query execution

## Features

- Geospatial interpolation for any request instead of relying only on fixed observation locations
- Better customer quality of life, customization, and product features

### Data Sources

- Multiple datasets (historical + forecast)
- Customer datasets and data blending

## Automation

- Downloader service
- Delta jobs
- Cron jobs for derived tables (hour, day, month, year)
## Production readiness
Scalability, multi-tenancy, reliability, sercurity... All standards to provide a real Saas product.
