using SMGeoDataContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SMGeo
{
    [ServiceContract]
    public interface ISMGeoServ
    {
        [OperationContract]
        bool IsAlive();

        [OperationContract]
        List<CountryData> GetCountriesList();

        [OperationContract]
        List<CityData> GetCitiesCollection(List<int> idCollection);
        [OperationContract]
        List<CountryData> GetCountriesCollection(List<int> idCollection);
        [OperationContract]
        List<CityData> GetCityList(int IDGeoCat, String GeoCategory);
        [OperationContract]
        CityData GetCityData(int IDCity);
        [OperationContract]
        List<StateRegionData> GetStatesList(int IDCountry);
        [OperationContract]
        List<GEOTimeZoneInfo> GetTimeZonesList();
        [OperationContract]
        List<StateRegionDataQueryRez> SearchStates(StateQueryData sqd);
        [OperationContract]
        StateRegionData GetStateData(int IDState);
        [OperationContract]
        CountryData GetCountryByState(int IDState);
        [OperationContract]
        List<StateRegionDataQueryRez> SearchEmptyStates();

        ///SAVE
        [OperationContract]
        int SaveCity(CityData cd);
        [OperationContract]
        int SaveCountry(CountryData cd);
        [OperationContract]
        int SaveStateRegion(StateRegionData cd);

        //DELETE
        [OperationContract]
        int DeleteCity(int ID);
        [OperationContract]
        int DeleteState(int ID);
        [OperationContract]
        int DeleteCountry(int ID);

        /// <Tests>
        /// /////////////////////////////////////////
        /// 
        [OperationContract]
        bool TestSaveDeleteCityStateCountry();

        //-----   -----  SELECT  -----  ------
        [OperationContract]
        bool TestSearchStates();
        [OperationContract]
        bool TestGetCitiesCollection();
        [OperationContract]
        bool TestGetCountriesCollection();
        [OperationContract]
        bool TestGetStatesList();
        [OperationContract]
        bool TestSearchCities();
    }
}
