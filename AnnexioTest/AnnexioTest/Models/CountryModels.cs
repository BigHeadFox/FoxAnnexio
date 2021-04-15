using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AnnexioTest.Models
{
    public class CountryModels
    {
    }

    public class AllCountriesModel
    {
        public string Name { get; set; }
        public string Region { get; set; }
        public string Subregion { get; set; }
    }

    public class CountryModel
    {
        public string sName { get; set; }
        public string sCapital { get; set; }
        public int iPopulation { get; set; }
        public List<string> lstCurrencies { get; set; }
        public List<string> lstLanguages { get; set; }
        public List<string> lstNeighbours { get; set; }
    }

    public class RegionModel
    {
        public string sRegion { get; set; }
        public int iPopulation { get; set; }
        public List<string> lstCountries { get; set; }
        public List<string> lstSubregions { get; set; }
    }

    public class SubRegionModel
    {
        public string sSubregion { get; set; }
        public int iPopulation { get; set; }
        public string sRegion { get; set; }
        public List<string> lstSubregionCountries { get; set; }
    }
}