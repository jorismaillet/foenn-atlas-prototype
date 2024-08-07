using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Assets.Scripts
{
    public class DepartmentRanking
    {
        public int department;
        public List<PostRanking> ranking;

        public DepartmentRanking(int department, List<PostRanking> ranking)
        {
            this.department = department;
            this.ranking = ranking;
        }
    }
}