using NostraDataService;
using SMGeoDataContracts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace SMGeo
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class SMGeoServ : ISMGeoServ
    {
        #region PROPERTIES
        private LogMaster _logmaster;
        protected LogMaster LM
        {
            get
            {
                if (_logmaster == null)
                    _logmaster = new LogMaster();

                return _logmaster;
            }
        }
        public bool IsTest { get; set; }
        #endregion
        #region CONSTRUCTORS
        public SMGeoServ()
        {
            IsTest = false;
        }


        #endregion
        #region GET
        public bool IsAlive()
        {
            return true;
        }
        public List<CountryData> GetCountriesList()
        {
            List<CountryData> lst = null;
            DataSet ds = null;
            try
            {
                lst = new List<CountryData>();
                ds = Utilities.GetDataSetbySQL(GetConnection(), "SELECT * FROM [geo].[countries] WHERE ID>0 Order By CountryName");
                if (IsDataSetValid(ds))
                {

                    lst = FromDataSetToListObj<CountryData>(ds);

                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("GetCountriesList", ex.Message);

            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return lst;
        }
        public List<CityData> GetCitiesCollection(List<int> idCollection)
        {
            List<CityData> lst = new List<CityData>();
            DataSet ds = null;
            if (idCollection.Count > 0)
            {
                try
                {
                    String SQL = "SELECT * FROM [geo].[vwCityData] WHERE ";
                    foreach (int id in idCollection)
                    {
                        SQL = String.Format("{0} PlaceID={1} OR", SQL, id);
                    }
                    SQL = SQL.Substring(0, SQL.Length - 2);
                    ds = Utilities.GetDataSetbySQL(GetConnection(), SQL);
                    if (IsDataSetValid(ds))
                    {
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            CityData cd = FromDataRowToObj<CityData>(dr);
                            cd.CountryData = FromDataRowToObj<CountryData>(dr);
                            cd.StateRegionData = FromDataRowToObj<StateRegionData>(dr);
                            lst.Add(cd);
                        }
                    }
                }
                catch (Exception ex)
                {
                    FormatAndLogMessage("getCityList", ex.Message);
                    if (ds != null)
                        ds.Dispose();

                }
                finally
                {
                    if (ds != null)
                        ds.Dispose();
                }
            }
            return lst;
        }
        public List<CountryData> GetCountriesCollection(List<int> idCollection)
        {
            List<CountryData> lst = null;
            DataSet ds = null;
            try
            {

                String SQL = "SELECT ID as CountryID, CountryName, Acronim as CountryAcr FROM [geo].[countries] WHERE ";
                foreach (int id in idCollection)
                {
                    SQL = String.Format("{0} ID={1} OR", SQL, id);
                }
                SQL = SQL.Substring(0, SQL.Length - 2);
                ds = Utilities.GetDataSetbySQL(GetConnection(), SQL);
                if (IsDataSetValid(ds))
                {
                    lst = FromDataSetToListObj<CountryData>(ds);
                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("GetCountriesList", ex.Message);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return lst;
        }
        public List<CityData> GetCityList(int IDGeoCat, String GeoCategory)
        {
            List<CityData> lst = new List<CityData>();
            SqlDataReader reader = null;
            HookUp hook = null;
            String sCategory = String.Equals(GeoCategory, "C", StringComparison.OrdinalIgnoreCase) ? "Country" : "State";
            try
            {
                DataSet ds = Utilities.GetDataSetbySQL(GetConnection(), String.Format("SELECT * FROM [geo].[vwCityData] WHERE {0}ID={1} ORDER BY PlaceName", sCategory, IDGeoCat));
                if (IsDataSetValid(ds))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        CityData cd = FromDataRowToObj<CityData>(dr);
                        cd.CountryData = FromDataRowToObj<CountryData>(dr);
                        cd.StateRegionData = FromDataRowToObj<StateRegionData>(dr);
                        lst.Add(cd);
                    }
                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("getCityList", ex.Message);
                if (hook != null)
                    hook.Close();
                if (reader != null)
                    reader.Dispose();
            }
            finally
            {
                if (hook != null)
                    hook.Close();
                if (reader != null)
                    reader.Dispose();
            }
            return lst;
        }
        public CityData GetCityData(int IDCity)
        {
            DataSet ds = null;
            CityData cd = null;
            try
            {
                ds = Utilities.GetDataSetbySQL(GetConnection(), String.Format("SELECT * FROM [geo].[vwCityData] WHERE PlaceID={0}", IDCity));
                if (IsDataSetValid(ds))
                {
                    cd = FromDataRowToObj<CityData>(ds.Tables[0].Rows[0]);
                    cd.CountryData = FromDataRowToObj<CountryData>(ds.Tables[0].Rows[0]);
                    cd.StateRegionData = FromDataRowToObj<StateRegionData>(ds.Tables[0].Rows[0]);

                }
            }
            catch (Exception ex)
            {

                FormatAndLogMessage("GetCityData", ex.Message);

            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return cd;
        }
        public List<StateRegionData> GetStatesList(int IDCountry)
        {
            List<StateRegionData> lst = null;
            DataSet ds = null;
            try
            {
                lst = new List<StateRegionData>();
                ds = Utilities.GetDataSetbySQL(GetConnection(), String.Format("SELECT StateAcr,[Country_ref], State_region,StateID FROM [geo].[vwStatesRegions] WHERE Country_ref={0} ORDER BY State_region", IDCountry));
                if (IsDataSetValid(ds))
                {

                    lst = FromDataSetToListObj<StateRegionData>(ds);
                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("GetStatesList", ex.Message);

                if (ds != null)
                    ds.Dispose();

            }
            return lst;
        }
        //PlaceSQLOperator = STARTS, EQUAL 
        //PlaceLogOperator = '' or 'NOT'
        //CountryIDLogOperator = ' or 'NOT'
        //StateIDLogOperator= ' or 'NOT'
        public List<CityData> SearchCities(CityQueryData cqd)
        {
            List<CityData> lstOut = null;
            DataSet ds = null;
            try
            {
                if (cqd != null)
                {
                    List<SqlParameter> lst = new List<SqlParameter>();

                    lst.Add(new SqlParameter("@PlaceName", cqd.PlaceName == null ? String.Empty : cqd.PlaceName));
                    lst.Add(new SqlParameter("@PlaceSQLOperator", cqd.PlaceSQLOperator == null ? String.Empty : cqd.PlaceSQLOperator));
                    lst.Add(new SqlParameter("@PlaceLogOperator", cqd.PlaceLogOperator == null ? String.Empty : cqd.PlaceLogOperator));
                    lst.Add(new SqlParameter("@CountryID", cqd.CountryID));
                    lst.Add(new SqlParameter("@CountryIDLogOperator", cqd.CountryIDLogOperator == null ? String.Empty : cqd.CountryIDLogOperator));
                    lst.Add(new SqlParameter("@StateID", cqd.StateID));
                    lst.Add(new SqlParameter("@StateIDLogOperator", cqd.StateIDLogOperator == null ? String.Empty : cqd.StateIDLogOperator));
                    ds = Utilities.GetDataSet(Constants.MainCon, "[geo].[SearchCities]", lst);

                    if (IsDataSetValid(ds))
                    {
                        lstOut = new List<CityData>();
                        foreach (DataRow dr in ds.Tables[0].Rows)
                        {
                            CityData cd = FromDataRowToObj<CityData>(dr);
                            cd.CountryData = FromDataRowToObj<CountryData>(dr);
                            cd.StateRegionData = FromDataRowToObj<StateRegionData>(dr);
                            lstOut.Add(cd);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("SearchCities", ex.Message);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return lstOut;
        }

        public List<GEOTimeZoneInfo> GetTimeZonesList()
        {
            List<GEOTimeZoneInfo> lst = null;
            DataSet ds = null;
            try
            {
                lst = new List<GEOTimeZoneInfo>();
                ds = Utilities.GetDataSetbySQL(GetConnection(), "SELECT * FROM [geo].[TimeZoneList] Order By TimeOffset");
                if (IsDataSetValid(ds))
                {
                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        lst = FromDataSetToListObj<GEOTimeZoneInfo>(ds);
                    }
                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("GetTimeZonesList", ex.Message);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return lst;
        }
        public List<StateRegionDataQueryRez> SearchStates(StateQueryData sqd)
        {
            List<StateRegionDataQueryRez> lstOut = null;
            DataSet ds = null;
            try
            {

                List<SqlParameter> lst = new List<SqlParameter>();

                lst.Add(new SqlParameter("@StateName", sqd.StateRegionName == null ? String.Empty : sqd.StateRegionName));
                lst.Add(new SqlParameter("@StateSQLOperator", sqd.StateSQLOperator == null ? String.Empty : sqd.StateSQLOperator));
                lst.Add(new SqlParameter("@StateLogOperator", sqd.StateLogOperator == null ? String.Empty : sqd.StateLogOperator));
                lst.Add(new SqlParameter("@CountryID", sqd.CountryID));
                lst.Add(new SqlParameter("@CountryIDLogOperator", sqd.CountryIDLogOperator == null ? String.Empty : sqd.CountryIDLogOperator));
                lst.Add(new SqlParameter("@Acronym", sqd.Acronym));
                lst.Add(new SqlParameter("@AcronymLogOperator", sqd.AcronymLogOperator == null ? String.Empty : sqd.AcronymLogOperator));
                lst.Add(new SqlParameter("@AcronymSQLOperator", sqd.AcronymSQLOperator == null ? String.Empty : sqd.AcronymSQLOperator));
                ds = Utilities.GetDataSet(Constants.MainCon, "[geo].[SearchStatesRegions]", lst);

                if (IsDataSetValid(ds))
                {
                    lstOut = FromDataSetToListObj<StateRegionDataQueryRez>(ds);
                }

            }
            catch (Exception ex)
            {
                FormatAndLogMessage("SearchStates", ex.Message);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }

            return lstOut;
        }
        public StateRegionData GetStateData(int IDState)
        {
            StateRegionData srd = new StateRegionData();
            DataSet ds = null;

            try
            {
                ds = Utilities.GetDataSetbySQL(GetConnection(), String.Format("SELECT * FROM [geo].[vwStatesRegions] WHERE StateID={0}", IDState));
                if (IsDataSetValid(ds))
                {
                    srd = FromDataRowToObj<StateRegionData>(ds.Tables[0].Rows[0]);
                }

            }
            catch (Exception ex)
            {

                FormatAndLogMessage("GetStateData", ex.Message);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return srd;
        }
        public CountryData GetCountryByState(int IDState)
        {
            CountryData cd = null;
            DataSet ds = null;
            try
            {
                List<SqlParameter> lst = new List<SqlParameter>();
                lst.Add(new SqlParameter("@StateId", IDState));
                ds = Utilities.GetDataSet(GetConnection(), "[geo].[GetCountryByState]", lst);
                if (IsDataSetValid(ds))
                {
                    cd = FromDataRowToObj<CountryData>(GetRow(ds));
                }
            }
            catch (Exception ex)
            {

                FormatAndLogMessage("GetCountryByState", ex.Message);

            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }
            return cd;
        }
        public List<StateRegionDataQueryRez> SearchEmptyStates()
        {
            List<StateRegionDataQueryRez> lstOut = null;
            DataSet ds = null;

            try
            {
                ds = Utilities.GetDataSet(Constants.MainCon, "[geo].[SearchStatesWithoutCities]");


                if (IsDataSetValid(ds))
                {
                    lstOut = FromDataSetToListObj<StateRegionDataQueryRez>(ds);
                }

            }
            catch (Exception ex)
            {
                FormatAndLogMessage("SearchStatesWithoutCities", ex.Message);
            }
            finally
            {
                if (ds != null)
                    ds.Dispose();
            }

            return lstOut;

        }
        #endregion
        #region SAVE
        public int SaveCity(CityData cd)
        {
            String Con = GetConnection();
            int iErr = -1;
            try
            {
                if (cd != null && !String.IsNullOrEmpty(cd.PlaceName) && cd.CountryData != null &&
                    cd.CountryData.CountryID > 0
                )
                {
                    List<SqlParameter> lst = new List<SqlParameter>();
                    lst.Add(new SqlParameter("@PlaceName", cd.PlaceName));
                    lst.Add(new SqlParameter("@Latitude", cd.Latitude));
                    lst.Add(new SqlParameter("@Longitude", cd.Longitude));
                    lst.Add(new SqlParameter("@Diff_Time", cd.Diff_Time));
                    lst.Add(new SqlParameter("@IDCountry", cd.CountryData.CountryID));
                    if (cd.StateRegionData != null && cd.StateRegionData.StateID > 0)
                        lst.Add(new SqlParameter("@IDState", cd.StateRegionData.StateID));
                    if (cd.PlaceID > 0)
                        lst.Add(new SqlParameter("@IDExisting", cd.PlaceID));

                    SqlParameter pErr = new SqlParameter("@ERR", iErr);
                    pErr.Direction = ParameterDirection.Output;
                    lst.Add(pErr);
                    SqlParameter pNewId = new SqlParameter("@NewID", cd.PlaceID);
                    pNewId.Direction = ParameterDirection.Output;
                    lst.Add(pNewId);
                    Utilities.ExecNonQuery(Con, "[geo].[SaveCity]", lst.ToArray());
                    iErr = Convert.ToInt16(pErr.Value);
                    cd.PlaceID = Convert.ToInt32(pNewId.Value);
                }
            }
            catch (Exception ex)
            {
                FormatAndLogMessage("SaveCity", ex.Message);
            }
            if (Con == Constants.MainCon)
                return iErr;
            else
                return cd.PlaceID;

        }
        public int SaveCountry(CountryData cd)
        {
            String Con = GetConnection();
            int iErr = -1;
            int ID = 0;
            if (cd != null && !String.IsNullOrEmpty(cd.CountryName) && !String.IsNullOrEmpty(cd.CountryName))
            {
                try
                {

                    List<SqlParameter> lst = new List<SqlParameter>();
                    lst.Add(new SqlParameter("@CountryName", cd.CountryName));
                    lst.Add(new SqlParameter("@Acronim", cd.CountryAcr));
                    lst.Add(new SqlParameter("@IDExisting", cd.CountryID));

                    SqlParameter pErr = new SqlParameter("@ERR", iErr);
                    pErr.Direction = ParameterDirection.Output;
                    lst.Add(pErr);
                    SqlParameter pNewId = new SqlParameter("@NewID", cd.CountryID);
                    pNewId.Direction = ParameterDirection.Output;
                    lst.Add(pNewId);
                    Utilities.ExecNonQuery(Con, "[geo].[SaveCountry]", lst.ToArray());
                    iErr = Convert.ToInt16(pErr.Value);
                    ID = Convert.ToInt32(pNewId.Value);
                    cd.CountryID = ID;
                }
                catch (Exception ex)
                {

                    FormatAndLogMessage("SaveCountry", ex.Message);
                }
                finally
                {

                }
            }
            if (Con == Constants.MainCon)
                return iErr;
            else
                return ID;
        }
        public int SaveStateRegion(StateRegionData cd)
        {
            String Con = GetConnection();
            int iErr = -1;
            int ID = 0;
            if (cd != null && !String.IsNullOrEmpty(cd.State_region) && !String.IsNullOrEmpty(cd.StateAcr) && cd.Country_ref > 0)
            {
                try
                {

                    List<SqlParameter> lst = new List<SqlParameter>();
                    lst.Add(new SqlParameter("@CountryID", cd.Country_ref));
                    lst.Add(new SqlParameter("@Acronim", cd.StateAcr));
                    lst.Add(new SqlParameter("@IDExisting", cd.StateID));
                    lst.Add(new SqlParameter("@StateName", cd.State_region));
                    SqlParameter pErr = new SqlParameter("@ERR", iErr);
                    pErr.Direction = ParameterDirection.Output;
                    lst.Add(pErr);
                    SqlParameter pNewId = new SqlParameter("@NewID", cd.StateID);
                    pNewId.Direction = ParameterDirection.Output;
                    lst.Add(pNewId);
                    Utilities.ExecNonQuery(Con, "[geo].[SaveStateRegion]", lst.ToArray());
                    iErr = Convert.ToInt16(pErr.Value);
                    ID = Convert.ToInt32(pNewId.Value);
                    cd.StateID = ID;
                }
                catch (Exception ex)
                {

                    FormatAndLogMessage("SaveStateRegion", ex.Message);
                }
                finally
                {

                }
            }
            if (Con == Constants.MainCon)
                return iErr;
            else
                return ID;
        }
        #endregion
        #region Update

        #endregion
        #region DELETE
        public int DeleteCity(int ID)
        {
            return Utilities.ExecNonQuery(GetConnection(), String.Format("DELETE FROM [geo].[cities] WHERE ID={0}", ID));
        }
        public int DeleteState(int ID)
        {
            return Utilities.ExecNonQuery(GetConnection(), String.Format("DELETE FROM [geo].[state_region] WHERE ID={0}", ID));

        }
        public int DeleteCountry(int ID)
        {
            return Utilities.ExecNonQuery(GetConnection(), String.Format("DELETE FROM [geo].[countries] WHERE ID={0}", ID));

        }
        #endregion


        #region NON Web Methods

        internal DataRow GetRow(DataSet ds)
        {
            return ds.Tables[0].Rows[0];
        }
        internal void FormatAndLogMessage(String sFunction, String sMsg)
        {
            String smsg = String.Format("SMGeo::{0}   {1:dd/MM/yy HH:mm}--> {2}", sFunction, DateTime.Now, sMsg);
            LM.SetLog(smsg);
        }
        internal bool IsDataSetValid(DataSet ds)
        {
            bool bRez = false;
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                bRez = true;

            return bRez;
        }
        internal string GetConnection()
        {
            return IsTest ? Constants.TestCon : Constants.MainCon;
        }
        internal List<T> FromDataSetToListObj<T>(DataSet ds)
        {
            List<T> lst = new List<T>();
            if (IsDataSetValid(ds))
            {
                lst = FromDataTableToListObj<T>(ds.Tables[0]);
            }
            return lst;
        }
        internal List<T> FromDataTableToListObj<T>(DataTable dt)
        {
            List<T> lst = new List<T>();

            foreach (DataRow dr in dt.Rows)
            {
                T objNew = FromDataRowToObj<T>(dr);
                new SMPropertiesUpdater<T>(dr, objNew);
                lst.Add(objNew);

            }

            return lst;
        }
        internal T FromDataRowToObj<T>(DataRow dr)
        {
            T objNew = (T)Activator.CreateInstance(typeof(T));
            new SMPropertiesUpdater<T>(dr, objNew);

            return objNew;
        }
        #endregion
        #region Tests
        public bool TestSaveDeleteCityStateCountry()
        {
            IsTest = true;
            bool bRez = true; ;
            CountryData contrData = new CountryData()
            {
                CountryName = "TestCountryName",
                CountryAcr = "TCN"
            };
            int IDCountry = SaveCountry(contrData);

            StateRegionData srd = new StateRegionData()
            {
                Country_ref = IDCountry,
                State_region = "TestsStateRegion",
                StateAcr = "TSR"
            };
            int IDState = SaveStateRegion(srd);
            CityData cd = new CityData()
            {
                CountryData = contrData,
                StateRegionData = srd,
                Diff_Time = -11.5,
                Latitude = 11.11,
                Longitude = -22.22,
                PlaceName = "TestPlace"
            };
            int IDCity = SaveCity(cd);
            if (IDCity > 0)
            {
                //TEST UPDATE
                cd.PlaceName = "TestPlace2";
                cd.Diff_Time = -12.5;
                cd.Latitude = 22.22;
                cd.Longitude = -33.33;
                IDCity = SaveCity(cd);

                CityData cd2 = GetCityData(cd.PlaceID);
                if (cd2 != null && cd2.Latitude == 22.22 && cd2.Longitude == -33.33 && cd2.Diff_Time == -12.5 &&
                cd2.StateRegionData.StateID == IDState && cd2.CountryData.CountryID == IDCountry)
                {
                    int iRez = DeleteCity(cd2.PlaceID);
                    cd2 = GetCityData(cd.PlaceID);
                    if (cd2 != null)
                        bRez = false;
                }
            }
            else
                bRez = false;

            if (IDState > 0)
            {
                DeleteState(IDState);
                srd = GetStateData(IDState);
                if (!(srd.Country_ref == 0 && String.IsNullOrEmpty(srd.StateAcr) &&
                    String.IsNullOrEmpty(srd.State_region)))
                    bRez = false;
            }
            if (IDCountry > 0)
            {
                DeleteCountry(IDCountry);
                List<int> collectID = new List<int>();
                collectID.Add(IDCountry);
                List<CountryData> lst = GetCountriesCollection(collectID);
                bRez = bRez && (lst == null || lst.Count == 0 ? true : false);
            }
            return bRez;
        }


        //-----   -----  SELECT  --
        public bool TestSearchStates()
        {
            StateQueryData sqd = new StateQueryData();
            //Test1 Place Operator
            List<StateRegionDataQueryRez> lstCD = null;

            //Test 1 
            sqd.StateRegionName = "Wa";
            sqd.StateSQLOperator = "LIKE";

            bool testRez = false;
            lstCD = SearchStates(sqd);
            if (lstCD != null &&
                !String.IsNullOrEmpty(lstCD[0].State_region) && !String.IsNullOrEmpty(lstCD[0].StateAcr) &&
                lstCD[0].StateID > 0 && !String.IsNullOrEmpty(lstCD[0].CountryName) && lstCD[0].Country_ref > 0
                )
                testRez = true;
            if (testRez)
            {
                //Test 2
                sqd.StateRegionName = "Wa";
                sqd.StateSQLOperator = "STARTS";

                testRez = false;
                lstCD = SearchStates(sqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].State_region) && !String.IsNullOrEmpty(lstCD[0].StateAcr) &&
                    lstCD[0].StateID > 0 && !String.IsNullOrEmpty(lstCD[0].CountryName) && lstCD[0].Country_ref > 0
                    )
                    testRez = true;
            }
            if (testRez)
            {
                //Test 3
                sqd.StateRegionName = "Wa";
                sqd.StateSQLOperator = "LIKE";
                sqd.CountryID = 5;
                sqd.CountryIDLogOperator = "OR";

                testRez = false;
                lstCD = SearchStates(sqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].State_region) && !String.IsNullOrEmpty(lstCD[0].StateAcr) &&
                    lstCD[0].StateID > 0 && !String.IsNullOrEmpty(lstCD[0].CountryName) && lstCD[0].Country_ref > 0
                    )
                    testRez = true;
            }
            if (testRez)
            {
                //Test 4
                sqd.StateRegionName = "Wa";
                sqd.StateSQLOperator = "LIKE";
                sqd.CountryID = 10;
                sqd.CountryIDLogOperator = "AND";

                testRez = false;
                lstCD = SearchStates(sqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].State_region) && !String.IsNullOrEmpty(lstCD[0].StateAcr) &&
                    lstCD[0].StateID > 0 && !String.IsNullOrEmpty(lstCD[0].CountryName) && lstCD[0].Country_ref > 0
                    )
                    testRez = true;
            }
            if (testRez)
            {
                //Test 5
                sqd.Acronym = "SK";
                sqd.AcronymSQLOperator = "EQUAL";
                testRez = false;
                lstCD = SearchStates(sqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].State_region) && !String.IsNullOrEmpty(lstCD[0].StateAcr) &&
                    lstCD[0].StateID > 0 && !String.IsNullOrEmpty(lstCD[0].CountryName) && lstCD[0].Country_ref > 0
                    )
                    testRez = true;
            }
            if (testRez)
            {
                //Test 6
                sqd.Acronym = "SK";
                sqd.AcronymSQLOperator = "STARTS";
                sqd.CountryID = 5;
                sqd.CountryIDLogOperator = "OR";
                testRez = false;
                lstCD = SearchStates(sqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].State_region) && !String.IsNullOrEmpty(lstCD[0].StateAcr) &&
                    lstCD[0].StateID > 0 && !String.IsNullOrEmpty(lstCD[0].CountryName) && lstCD[0].Country_ref > 0
                    )
                    testRez = true;
            }
            if (testRez)
            {
                //Test 6
                sqd.Acronym = "SK";
                sqd.AcronymSQLOperator = "STARTS";
                sqd.CountryID = 5;
                sqd.CountryIDLogOperator = "AND";
                testRez = false;
                lstCD = SearchStates(sqd);
                if (lstCD == null || lstCD.Count == 0)
                    testRez = true;
            }
            return testRez;
        }
        public bool TestSearchCities()
        {
            CityQueryData cqd = new CityQueryData();
            //Test1 Place Operator
            List<CityData> lstCD = null;
            cqd.PlaceName = "Mo";
            cqd.PlaceSQLOperator = "STARTS";

            bool testRez = false;
            lstCD = SearchCities(cqd);
            if (lstCD != null &&
                !String.IsNullOrEmpty(lstCD[1].PlaceName) && !String.IsNullOrEmpty(lstCD[1].StateRegionData.StateAcr) &&
                lstCD[1].PlaceID > 0
                )
                testRez = true;
            //Test 2 PlaceName Equal "Montreal"
            if (testRez)
            {
                cqd = new CityQueryData();
                cqd.PlaceName = "Montreal";
                cqd.PlaceSQLOperator = "EQUAL";
                testRez = false;
                lstCD = SearchCities(cqd);
                if (lstCD != null && lstCD.Count == 1 &&
                    !String.IsNullOrEmpty(lstCD[0].PlaceName) && !String.IsNullOrEmpty(lstCD[0].StateRegionData.StateAcr) &&
                    lstCD[0].PlaceID > 0
                    )
                    testRez = true;
            }
            //Test 2 PlaceName Starts "Mo" and CountryId=10
            if (testRez)
            {
                cqd = new CityQueryData();
                cqd.PlaceName = "Mo";
                cqd.PlaceSQLOperator = "STARTS";
                cqd.CountryID = 10;
                cqd.CountryIDLogOperator = "AND";
                testRez = false;
                lstCD = SearchCities(cqd);
                if (lstCD != null && lstCD.Count == 1 &&
                    !String.IsNullOrEmpty(lstCD[0].PlaceName) && !String.IsNullOrEmpty(lstCD[0].StateRegionData.StateAcr) &&
                    lstCD[0].PlaceID > 0
                    )
                    testRez = true;
            }
            //Test 3 PlaceName Starts "Mo" and StateID=5
            if (testRez)
            {
                cqd = new CityQueryData();
                cqd.PlaceName = "Mo";
                cqd.PlaceSQLOperator = "STARTS";
                cqd.StateID = 5;
                cqd.StateIDLogOperator = "AND";
                testRez = false;
                lstCD = SearchCities(cqd);
                if (lstCD != null && lstCD.Count == 1 &&
                    !String.IsNullOrEmpty(lstCD[0].PlaceName) && !String.IsNullOrEmpty(lstCD[0].StateRegionData.StateAcr) &&
                    lstCD[0].PlaceID > 0
                    )
                    testRez = true;
            }
            //Test 4 PlaceName Starts "Mos" OR StateID=5
            if (testRez)
            {
                cqd = new CityQueryData();
                cqd.PlaceName = "Mos";
                cqd.PlaceSQLOperator = "STARTS";
                cqd.CountryID = 5;
                cqd.CountryIDLogOperator = "OR";
                testRez = false;
                lstCD = SearchCities(cqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].PlaceName) && !String.IsNullOrEmpty(lstCD[0].StateRegionData.StateAcr) &&
                    lstCD[0].PlaceID > 0
                    )
                    testRez = true;
            }
            //Test 5 PlaceName Starts "Mo" OR CountryId=5
            if (testRez)
            {
                cqd = new CityQueryData();
                cqd.PlaceName = "Mo";
                cqd.PlaceSQLOperator = "STARTS";
                cqd.CountryID = 5;
                cqd.CountryIDLogOperator = "OR";
                testRez = false;
                lstCD = SearchCities(cqd);
                if (lstCD != null &&
                    !String.IsNullOrEmpty(lstCD[0].PlaceName) && !String.IsNullOrEmpty(lstCD[0].StateRegionData.StateAcr) &&
                    lstCD[0].PlaceID > 0
                    )
                    testRez = true;
            }
            return testRez;
        }
        public bool TestGetStatesList()
        {
            int Id = 10;

            List<StateRegionData> lstCD = GetStatesList(Id);
            bool testRez = false;
            if (lstCD != null &&
                lstCD.Count == 6 &&
                !String.IsNullOrEmpty(lstCD[1].StateAcr) && !String.IsNullOrEmpty(lstCD[1].State_region) &&
                lstCD[1].Country_ref == 10
                )
                testRez = true;
            return testRez;
        }
        public bool TestGetCountriesCollection()
        {
            List<int> lst = new List<int>();
            int iCount = 5;
            lst.AddRange(Enumerable.Range(0, iCount));

            List<CountryData> lstCD = GetCountriesCollection(lst);
            bool testRez = false;
            if (lstCD != null &&
                lstCD.Count == iCount &&
                !String.IsNullOrEmpty(lstCD[1].CountryAcr) && !String.IsNullOrEmpty(lstCD[1].CountryName) &&
                lstCD[1].CountryID == 1
                )
                testRez = true;
            return testRez;
        }
        public bool TestGetCitiesCollection()
        {
            List<int> lst = new List<int>();
            lst.Add(2);
            lst.Add(3);
            lst.Add(4);
            lst.Add(12);
            List<CityData> lstCD = GetCitiesCollection(lst);
            bool testRez = false;
            if (lstCD != null &&
                lstCD.Count == 4 &&
                lstCD[3].CountryData != null && lstCD[3].CountryData.CountryAcr == "RU" && lstCD[3].CountryData.CountryName == "Russia" &&
                lstCD[3].StateRegionData != null && lstCD[3].StateRegionData.StateID == 2 && lstCD[3].StateRegionData.StateAcr == "Url" && lstCD[3].StateRegionData.State_region == "Ural"
                )
                testRez = true;
            return testRez;
        }
        #endregion


    }
}
