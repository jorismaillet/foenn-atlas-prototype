using Assets.Resources.Weathers;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts.Legacy.Results
{
    public class PostRanking
    {
        public WeatherPostDataset post;
        public int rank;

        public PostRanking(WeatherPostDataset post, int rank)
        {
            this.post = post;
            this.rank = rank;
        }
    }
}