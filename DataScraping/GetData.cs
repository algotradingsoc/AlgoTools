
#r "System.Configuration"
#r "System.Data"

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Configuration;
using System.Data.SqlClient;
using System.Threading.Tasks;



public struct dataret 	
{	/*
	Function contains all the data types returned by the free subscription to Quandl's LSE database.
	Setnull function initialises all values to an empty string for compilation purposes
	SetDate reformats Date to the corrent 'Date' format required for SQL server to recognise it
	*/
    public String Date;
    public String price;
    public String high;
    public String low;
    public String volume;
    public String last_close;
    public String change;
    public String varp;

    public void Setnull(){	
        Date = "";
        price = "";
        high = "";
        low = "";
        last_close = "";
        volume = "";
        change = "";
        varp = "";
    }
    public void SetDate(){
        Date = "\'" + Date + "\'"; 
    }
}



public static void Run(TimerInfo myTimer, TraceWriter log)
{
	//Main loop of the program, runs the main program for each Ticker stored in the DB
    List<String> Schemas = new List<String>();
    List<String> Tickers = new List<String>();
	
    GetTickers(log, ref Schemas, ref Tickers);      //calls GetTickers on Schemas and Tickers, fills them with the values from the database
    
    for(int i = 0; i < Schemas.Count; i++){			//for each value in Schemas and Tickers they are put through the program, the length of Schemas and Tickers will always be the same, therefore it doesn't matter which is chosen for the loop
													
        String data = GetData(Schemas[i], Tickers[i], log);		//calls the Quandl API for the values for each Ticker
        log.Info(data);
        try
		{ 										//throws error if the length of data is less than 7, this gives two different errors where no data at all is returned, or if the call itself threw an error
            if(data.Substring(0,6) == "Failed")		//ends runtime if valid datastring is not returned
            {   
               log.Info("Return is invalid");
            }
            else
            {
                dataret Data2 = new dataret();	
                ParseData(data, log, ref Data2);		//assigns Data2 the values returned from the API
                WriteUpdate(log, Data2, Schemas[i], Tickers[i]);		//writes the value of Data2 to the database
            }
        }
		catch
		{
            log.Info($"Return is invalid for {Schemas[i]}.{Tickers[i]}, breaking loop");
            break;
        }           
       
    }
}

public static void GetTickers (TraceWriter log, ref List<String> Schemas, ref List<String> Tickers)
{             
	//Gives Schemas and Tickers the values stored in the Tickers table in the database
    var str = ConfigurationManager.ConnectionStrings["ConnectSQL"].ConnectionString;		//grabs the connection string from the function setup information
    using (SqlConnection conn = new SqlConnection(str))		//creates SqlConnection object and initialises with the connection string
    {
        conn.Open();		
        String text = "SELECT Tickers, Exchanges FROM [dbo].[All Tickers]";
        using (SqlCommand cmd = new SqlCommand(text, conn))	//creates SqlCommand object with command and connection				
        {
            SqlDataReader reader = cmd.ExecuteReader();

            while(reader.Read()){
                Tickers.Add(reader.GetString(0));		//Gives Tickers and Schemas the values returned from the table
				Schemas.Add(reader.GetString(1));
            }
        }
        conn.Close();
    }
}

public static void WriteUpdate (TraceWriter log, dataret Data2, String schema, String table)
{
	//Writes a single new value to the appropriate table 
    var str = ConfigurationManager.ConnectionStrings["ConnectSQL"].ConnectionString; //grabs connection string from system files (in Azure; input connection string into "")
        
    using (SqlConnection conn = new SqlConnection(str))		//creates SqlConnection object and initialises with the connection string
    {
        conn.Open();		
        String text = $"INSERT INTO [{schema}].[{table}] VALUES ({Data2.Date},{Data2.price},{Data2.high},{Data2.low},{Data2.volume},{Data2.last_close},{Data2.change},{Data2.varp})";
		//^Creates String with the SQL insert command to write the values to the database
        using (SqlCommand cmd = new SqlCommand(text, conn))	//creates SqlCommand object with command and connection				
        {
            // Execute the command and log the # rows affected.
            
            var rows = cmd.ExecuteNonQuery();	//inputs command and awaits response
            log.Info($"{rows} rows were updated");		//confirms that the rows were updated
        }
    }
}

public static String GetData(String database_code, String dataset_code, TraceWriter log ) 
{	//Returns the previous day's data as a single CSV string, this currently only does this for a single company value. However, with a premium subscription this could be reduced to a single call by using multiple tickers per call
    DateTime dateNow = DateTime.UtcNow.AddDays(-1);		//ensures that data is only given for the previous day
    String currentDate = dateNow.ToString("yyyy-MM-dd");	

    String APIkey = "your_key_here";     //quandl API key should be inserted here
    String dataString = null; //initialises the return value 
    
    try
    {
        using (WebClient API = new WebClient())		//creates webclient object
        {
            String address = $"https://www.quandl.com/api/v3/datasets/{database_code}/{dataset_code}.csv?exclude_column_names=true&start_date={currentDate}&end_date={currentDate}api_key={APIkey}";
            //^produces the API call in the correct format
            log.Info(address);
            dataString = API.DownloadString(address);
			//calls the API
            return dataString;
        }
    }
    catch (Exception e)
    {
        return $"Failed: {e}";
    }
}
        
public static void ParseData(String Data, TraceWriter log, ref dataret parsed) //returns the CSV string as an array of 6 strings
{
	//changes the CSV data to a dataret object
    Data = Data + ","; //append ',' to the end of data to trigger the case statement correctly.
    parsed.Setnull(); //initialises values a blank
    int index = 0;	//keeps track of data
    StringBuilder sb = new StringBuilder();	
    foreach(char element in Data)  //runs through all characters in the CSV data and adds it to the dataret object
    {
        if(index <= 7) {
            if (element != ',')
            {
                sb.Append(element);
            }
            else
            {
                switch(index){
                    case 0:
                        parsed.Date = sb.ToString();
                        break;
                    case 1:
                        parsed.price = sb.ToString();
                        break;
                    case 2:
                        parsed.high = sb.ToString();
                        break;
                    case 3:
                        parsed.low = sb.ToString();
                        break;
                    case 4:
                        parsed.volume = sb.ToString();
                        break;
                    case 5:
                        parsed.last_close = sb.ToString();
                        break;
                    case 6:
                        parsed.change = sb.ToString();
                        break;
                    case 7:
                        parsed.varp = sb.ToString();
                        break;
                }
                sb.Clear();
                index++;
            }
        }
    }
    parsed.SetDate();	//converts the date value in the dataret object to the correct format
}
