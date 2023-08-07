﻿using FluentEmail.Core;
using Microsoft.EntityFrameworkCore;
using System.Runtime.CompilerServices;

namespace HealthyFoodWebsite.PaginationEngine
{
    public static class PaginationEngine
    {
        public static IEnumerable<List<T>> GetPages<T>(this IEnumerable<T> data, int itemsPerPage)
        {
            List<T>? listedData = data.ToList();
            int listedDataCount = listedData.Count;
            List<T> pageList = new();
            List<List<T>> pagesList = new();

            for(int i = 1; i <= listedDataCount; i++)
            {
                pageList.Add(listedData[i - 1]);

                if (i % itemsPerPage == 0 || i == listedDataCount)
                {

                    pagesList.Add(new List<T>(pageList));
                    pageList.Clear();
                }
            }

            return pagesList;
        }
    }
}
