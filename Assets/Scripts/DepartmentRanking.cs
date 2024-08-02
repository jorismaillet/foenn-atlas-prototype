using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class DepartmentRanking
    {
        public int department;
        public List<Tuple<string, int>> ranking;

        public DepartmentRanking(int department, List<Tuple<string, int>> ranking)
        {
            this.department = department;
            this.ranking = ranking;
        }
    }
}