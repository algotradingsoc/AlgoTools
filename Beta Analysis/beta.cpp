#include <stdio.h>
#include<iostream>
#include <math.h>
#include<string>
#include<fstream>

 using namespace std;

class StdDeviation

{

private:

    int max;

    double value[1259];

    double mean;

public:

    double CalculateMean()

    {

        double sum = 0;

        for(int i = 0; i < max; i++)

            sum += value[i];

        return (sum / max);

    }

 

    double CalculateVariance()

    {

        mean = CalculateMean();

 

        double temp = 0;

        for(int i = 0; i < max; i++)

        {

             temp += (value[i] - mean) * (value[i] - mean) ;

        }

        return temp / max;

    }

 

    double CalculateSampleVariane()

    {

        mean = CalculateMean();

 

        double temp = 0;

        for(int i = 0; i < max; i++)

        {

             temp += (value[i] - mean) * (value[i] - mean) ;

        }

        return temp / (max - 1);

    }

 

    int SetValues(double *p, int count)

    {
        max = count;

        for(int i = 0; i < count; i++)

            value[i] = p[i];

        return 0;

    }

 

    double Calculate_StandardDeviation()

    {

        return sqrt(CalculateVariance());

    }

 

    double Calculate_SampleStandardDeviation()

    {

        return sqrt(CalculateSampleVariane());

    }

 

};

 

class FinanceCalculator

{

private:

    double XSeries[1259];

    double YSeries[1259];

    int max;

 

    StdDeviation x;

    StdDeviation y;

 

public:

    void SetValues(double *xvalues, double *yvalues, int count)

    {
        for(int i = 0; i < count; i++)

        {

            XSeries[i] = xvalues[i];

            YSeries[i] = yvalues[i];

        }

        x.SetValues(xvalues, count);

        y.SetValues(yvalues, count);

        max = count;

    }

 

    double Calculate_Covariance()

    {

        double xmean = x.CalculateMean();

        double ymean = y.CalculateMean();

 

        double total = 0;

        for(int i = 0; i < max; i++)

        {

            total += (XSeries[i] - xmean) * (YSeries[i] - ymean);

        }

        return total / max;

    }

 

    double Calculate_Correlation()

    {

        double cov = Calculate_Covariance();

        double correlation = cov / (x.Calculate_StandardDeviation() * y.Calculate_StandardDeviation());

        return correlation;

    }
	
	double Calculate_variance_x()
	{
		double var= x.CalculateVariance();
		return var;
	}
	double Calculate_Beta()
	{
		double var= x.CalculateVariance();
		double cov = Calculate_Covariance();
		double beta = cov/var;
		return beta;
		
	}

};

 class File
 {
	public:
	double values[1260], returns[1259];
	double read_file(string file)
	{
		ifstream ist;
		ist.open(file);
		double close[1260];
		string trash;
		getline(ist, trash);
		int count = 0;
		while(count<1261){
			string temp_date, temp_close;
			double c_price;
			getline(ist, temp_date, ',');
			getline(ist, temp_close);
			close[count] = atof(temp_close.c_str());
			count+=1;
		}
		std::copy(std::begin(close), std::end(close), std::begin(values));
		ist.close();	
		
	}
	void Calculate_returns(){
		for(int i=0; i<1259; i++){
			returns[i] = 100*log(values[i+1]/values[i]);
		}
	}
 };


int main()

{
	File market;
	market.read_file("^GSPC.csv");
	market.Calculate_returns();
    FinanceCalculator calc; 
	File stock;
	stock.read_file("IBM.csv");
	stock.Calculate_returns();
	calc.SetValues(market.returns,stock.returns,sizeof(market.returns) / sizeof(market.returns[0]));
	cout<<"Covariance = "<<calc.Calculate_Covariance()<<endl;
	cout<<"Beta = "<<calc.Calculate_Beta()<<endl;
	cout<<stock.returns[0]<<endl;
	//TEST DATA

    /* {

        printf("\n\nZero Correlation and Covariance Data Set\n");

        double xarr[] = { 8, 6, 4, 6, 8 };

        double yarr[] = { 10, 12, 14, 16, 18 };

 

        calc.SetValues(xarr,yarr,sizeof(xarr) / sizeof(xarr[0]));

		cout<<"Covariance = "<<calc.Calculate_Covariance()<<endl;
		cout<<"Beta = "<<calc.Calculate_Beta()<<endl;


    }

 

    {

        printf("\n\nPositive Correlation and Low Covariance Data Set\n");

        double xarr[] = { 0, 2, 4, 6, 8 };

        double yarr[] = { 6, 13, 15, 16, 20 };

 

        calc.SetValues(xarr,yarr,sizeof(xarr) / sizeof(xarr[0]));

 

        cout<<"Covariance = "<<calc.Calculate_Covariance()<<endl;
		cout<<"Beta = "<<calc.Calculate_Beta()<<endl;

    }

    {

        printf("\n\nNegative Correlation and Low Covariance Data Set\n");

        double xarr[] = { 8, 6, 4, 2, 0 };

        double yarr[] = { 6, 13, 15, 16, 20 };

 

        calc.SetValues(xarr,yarr,sizeof(xarr) / sizeof(xarr[0]));

 

        cout<<"Covariance = "<<calc.Calculate_Covariance()<<endl;
		cout<<"Beta = "<<calc.Calculate_Beta()<<endl;

    }

 

    {

        printf("\n\nPositive Correlation and High Covariance Data Set\n");

        double xarr[] = { 8, 6, 4, 2, 0 };

        double yarr[] = { 1006, 513, 315, 216, 120 };

 

        calc.SetValues(xarr,yarr,sizeof(xarr) / sizeof(xarr[0]));

 

        cout<<"Covariance = "<<calc.Calculate_Covariance()<<endl;
		cout<<"Beta = "<<calc.Calculate_Beta()<<endl;

    }

    {

        printf("\n\nNegative Correlation and High Covariance Data Set\n");

        double xarr[] = { 8, 6, 4, 2, 0 };

        double yarr[] = { 120, 216, 315, 513, 1006 };

 

        calc.SetValues(xarr,yarr,sizeof(xarr) / sizeof(xarr[0]));

 

        cout<<"Covariance = "<<calc.Calculate_Covariance()<<endl;
		cout<<"Beta = "<<calc.Calculate_Beta()<<endl;

    } */
	return 0;

}