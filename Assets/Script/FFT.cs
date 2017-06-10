using System;

public   class FFT
{
	public FFT ()
	{
	}

	static int NFFT;
	static	double[] fftdata;
	static int length = 256;


	void  CalFFT(int isign)
	{
		int n, mmax, m, j, istep, i;
		double   wtemp, wr, wpr, wpi, wi, theta;
		double tempr, tempi;
		n = NFFT << 1;
		j = 1;
		for (i = 1; i < n; i += 2)
		{
			if (j > i)
			{
				tempr = fftdata[j]; fftdata[j] = fftdata[i]; fftdata[i] = tempr;
				tempr = fftdata[j + 1]; fftdata[j + 1] = fftdata[i + 1]; fftdata[i + 1] = tempr;
			}
			m = n >> 1;
			while (m >= 2 && j > m)
			{
				j -= m;
				m >>= 1;
			}
			j += m;
		}
		mmax = 2;
		while (n > mmax)
		{
			istep = 2 * mmax;
			theta = 2f * Math   .PI / (isign * mmax);
			wtemp = Math.Sin(0.5 * theta);
			wpr = -2.0f * wtemp * wtemp;
			wpi = Math.Sin (theta);
			wr = 1.0f;
			wi = 0.0f;
			for (m = 1; m < mmax; m += 2)
			{
				for (i = m; i <= n; i += istep)
				{
					j = i + mmax;
					tempr = wr * fftdata[j] - wi * fftdata[j + 1];
					tempi = wr * fftdata[j + 1] + wi * fftdata[j];
					fftdata[j] = fftdata[i] - tempr;
					fftdata[j + 1] = fftdata[i + 1] - tempi;
					fftdata[i] += tempr;
					fftdata[i + 1] += tempi;
				}
				wr = (wtemp = wr) * wpr - wi * wpi + wr;
				wi = wi * wpr + wtemp * wpi + wi;
			}
			mmax = istep;
		}
	}

	public void FFTManagerinit(int Nx,datafilter df)
	{
		if (Nx < 128) {
			Console.Write ("not enough data length");
		}
		if (Nx % 2 == 0) {
			length = Nx;
		} else {
			length = Nx - 1;
		}
		DF = df;
		NFFT = (int)Math .Pow(2.0f, Math .Ceiling (Math.Log10 ((float )Nx) / Math.Log10 (2.0f)));
		//NFFT=Mathf.NextPowerOfTwo(Nx );
		fftdata = new double[2*NFFT+1];
	}

	public float [] CalNFFT(float[] data)
	{
		
		if (Math.Abs ( data.Length- length)>1) {
			FFTManagerinit(data.Length,datafilter.none  );
		}


		int i = 0;
		for (i = 0; i < length ; i++)
		{
			fftdata[2*i+1] =(double ) data[i];
			fftdata[2*i+2] = 0.0f;
		}
		/* pad the remainder of the array with zeros (0 + 0 j) */
		for (i = length; i < NFFT; i++)
		{
			fftdata[2 * i + 1] = 0.0f;
			fftdata[2 * i + 2] = 0.0f;
		}

		CalFFT(1);
		float[] resultdata=new float [length /2];
		for (i = 0; i < length/2; i++)
		{

			switch (DF) {
			case datafilter.unityspec:
				resultdata [i] = (float)Math.Abs( fftdata [2 * i + 1]);
				break;
			case datafilter.none:
				resultdata[i] = (float)fftdata[2*i+1];
				break;
			}

			//resultdata[i] = (float)fftdata[2*i];
		}
		return resultdata;
	}

	public  enum datafilter{
		unityspec,none
	}

	datafilter DF;
	public  float[] windowRect(float[] data){
		int N = data.Length;
		float[] newdata=new float[N] ;
		for (int i = 0; i < (N+ 1) / 2; i++) {
			newdata [i] = data [i];
			newdata [N - 1 - i] = data [i];
		}
		return newdata;
	}

	public float[] windowBlackman(float[] data){
		int N = data.Length;
		float[] newdata=new float[N] ;
		for (int i = 0; i < (N+ 1) / 2; i++) {

			double  result=data [i]*(0.42-0.5*Math.Cos(Math.PI *2 *i/(N-1))+0.08*Math.Cos(Math.PI *4 *i/(N-1)));
			newdata [i] =(float)result;//data [i]*(0.42-0.5*Math.Cos(Math.PI *2 *i/(N-1))+0.08*Math.Cos(Math.PI *4 *i/(N-1)));
			newdata [N - 1 - i] =(float)result;// newdata [i];
		}
		//Debug.Log (teststr);
		return newdata;
	}

}


