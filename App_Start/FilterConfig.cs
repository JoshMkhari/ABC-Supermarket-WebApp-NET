﻿using System.Web.Mvc;

namespace _20104681JoshMkhariCLDV6212Task2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
