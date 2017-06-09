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

	public void FFTManagerinit(int Nx)
	{
		length = Nx;
		NFFT = (int)Math .Pow(2.0f, Math .Ceiling (Math.Log10 ((float )Nx) / Math.Log10 (2.0f)));
		//NFFT=Mathf.NextPowerOfTwo(Nx );
		fftdata = new double[2*NFFT+1];
	}

	public void CalNFFT(double[] data)
	{
		if (data.Length < length) {
			return;
		}
		int i = 0;
		for (i = 0; i < length ; i++)
		{
			fftdata[2*i+1] = data[i];
			fftdata[2*i+2] = 0.0f;
		}
		/* pad the remainder of the array with zeros (0 + 0 j) */
		for (i = length; i < NFFT; i++)
		{
			fftdata[2 * i + 1] = 0.0f;
			fftdata[2 * i + 2] = 0.0f;
		}

		CalFFT(1);

		for (i = 0; i < length; i++)
		{
			data[i] = fftdata[2*i+1];
		}
	}




}


