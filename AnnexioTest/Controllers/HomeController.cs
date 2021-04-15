using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web.Mvc;
using AnnexioTest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AnnexioTest.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            var json = GetJsonForAllCountries();
            var model = ListOfCountries(json);
            return View(model);
        }

        [HttpPost]
        public JsonResult ShowMoreInfo(string category, string value)
        {
            var json = GetMoreInfo(category, value);
            return Json(json);
        }

        [HttpPost]
        public JsonResult CountryCodeFromUser(string cCode)
        {
            var name = GetCountryNameFromCode(cCode);
            return Json(name);
        }

        protected string RawJSONFromURL(string url)
        {
            var webClient = new WebClient();
            var rawJson = webClient.DownloadString(url);            
            return rawJson;
        }

        protected string EncodeStringToUTF8(string sString)
        {
            byte[] bytes = Encoding.Default.GetBytes(sString);
            var s = Encoding.UTF8.GetString(bytes);
            return s;
        }

        protected string GetCleanJSONFromURL(string url)
        {
            var rawJSon = RawJSONFromURL(url);
            var json = EncodeStringToUTF8(rawJSon);
            return json;
        }

        protected dynamic GetDynamicObjectFromJson(string json)
        {
            dynamic data = JsonConvert.DeserializeObject(json);
            return data;
        }

        protected string GetJsonForAllCountries()
        {
            var json = string.Empty;
            // Cache it for 1.4 of the brief
            if (System.Web.HttpContext.Current.Cache["allCountries"] != null)
            {
                json = System.Web.HttpContext.Current.Cache["allCountries"].ToString();
            }
            else
            {
                json = GetCleanJSONFromURL("https://restcountries.eu/rest/v2/all");
                System.Web.HttpContext.Current.Cache["allCountries"] = json;
            }
            return json;
        }

        protected List<AllCountriesModel> ListOfCountries(string json)
        {
            var finalJSON = string.Empty;
            
            dynamic data = GetDynamicObjectFromJson(json);

            List<AllCountriesModel> lstCountries = new List<AllCountriesModel>();            

            for (int i = 0; i < data.Count; i++)
            {
                AllCountriesModel country = new AllCountriesModel();
                country.Name = data[i].name;
                country.Region = data[i].region;
                country.Subregion = data[i].subregion;
                lstCountries.Add(country);
            }

            return lstCountries;
        }

        protected string GetMoreInfo(string category, string value)
        {
            category = category.ToLower();
            value = value.ToLower();

            var url = "https://restcountries.eu/rest/v2/";
            var json = string.Empty;

            if (category == "name")
            {
                url += "name/" + value;
                json = GetCleanJSONFromURL(url);
                return GetCountryInfo(json);
            }
            else if (category == "region")
            {
                url += "region/" + value;
                json = GetCleanJSONFromURL(url);
                return GetRegionInfo(json);
            }
            else if (category == "subregion")
            {
                json = GetJsonForAllCountries();
                return GetSubregionInfo(json, value);
            }

            return string.Empty;    // Handle this better.
        }

        protected string GetCountryInfo(string json)
        {
            dynamic data = GetDynamicObjectFromJson(json);

            dynamic countryInfo = new JObject();
            countryInfo.Capital = data[0].capital;
            countryInfo.Population = data[0].population.ToString("N0");

            List<string> lstCurrencies = new List<string>();
            for (var i = 0; i < data[0].currencies.Count; i++)
            {
                lstCurrencies.Add(data[0].currencies[i].name.ToString() + " (" + data[0].currencies[i].symbol.ToString() + ")");
            }
            countryInfo.Currencies = string.Join(", ", lstCurrencies);

            List<string> lstLanguages = new List<string>();
            for (var i = 0; i < data[0].languages.Count; i++)
            {
                lstLanguages.Add(data[0].languages[i].name.ToString());
            }
            countryInfo.Languages = string.Join(", ", lstLanguages);

            List<string> lstBorders = new List<string>();
            for (var i = 0; i < data[0].borders.Count; i++)
            {
                lstBorders.Add("<span class='borderCountry'>" + data[0].borders[i].ToString() + "</span>");
            }
            countryInfo.Borders = string.Join(", ", lstBorders);

            return countryInfo.ToString();
        }

        protected string GetCountryNameFromCode(string cCode)
        {
            var rawJson = RawJSONFromURL("https://restcountries.eu/rest/v2/alpha/" + cCode);
            var json = EncodeStringToUTF8(rawJson);
            dynamic data = GetDynamicObjectFromJson(json);
            dynamic codeInfo = new JObject();
            codeInfo.Name = data.name;           

            return codeInfo.ToString();
        }

        protected string GetRegionInfo(string json)
        {
            dynamic data = GetDynamicObjectFromJson(json);

            dynamic regionInfo = new JObject();
            regionInfo.Name = data[0].region;
            var regionPopulation = 0;
            List<string> lstRegionCountries = new List<string>();
            List<string> lstSubregions = new List<string>();

            for (var i = 0; i < data.Count; i++)
            {
                regionPopulation += Convert.ToInt32(data[i].population);
                lstRegionCountries.Add("<span class='regionCountry'>" + data[i].name.ToString() + "</span>");

                if (!lstSubregions.Contains("<span class='subregionName'>" + data[i].subregion.ToString() + "</span>"))
                {
                    lstSubregions.Add("<span class='subregionName'>" + data[i].subregion.ToString() + "</span>");
                }
            }
            regionInfo.Population = regionPopulation.ToString("N0");
            regionInfo.Countries = string.Join(", ", lstRegionCountries);
            regionInfo.Subregions = string.Join(", ", lstSubregions);

            return regionInfo.ToString();
        }

        protected string GetSubregionInfo(string json, string subregionName)
        {
            dynamic data = GetDynamicObjectFromJson(json);
            dynamic subregionInfo = new JObject();

            int subregionPopulation = 0;
            var region = string.Empty;
            List<string> subregionCountries = new List<string>();

            for (var i = 0; i < data.Count; i++)
            {
                if (data[i].subregion.ToString().ToUpper() == subregionName.ToUpper())
                {
                    subregionPopulation += Convert.ToInt32(data[i].population.ToString());
                    region = "<span class='regionName'>" + data[i].region.ToString();   // TO FIX: Inefficient to keep updating the same value.
                    subregionCountries.Add("<span class='regionCountry'>" + data[i].name.ToString() + "</span>");
                }
            }

            TextInfo ti = new CultureInfo("en-GB", false).TextInfo;

            subregionInfo.Subregion = ti.ToTitleCase(subregionName);
            subregionInfo.Population = subregionPopulation.ToString("N0");
            subregionInfo.Region = region;
            subregionInfo.Countries = string.Join(", ", subregionCountries);

            return subregionInfo.ToString();
        }
    }
}