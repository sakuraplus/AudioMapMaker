using UnityEngine;
using System.Collections;

/// <summary>
/// audioprocessor的方法/// 
/// </summary>
public class maintest3ap : MonoBehaviour {
	public AudioSource audioSource;

	private long lastT, nowT, diff, entries, sum;

	public int bufferSize = 1024;
	// fft size
	private int samplingRate = 44100;
	// fft sampling frequency

	/* log-frequency averaging controls */
	private int nBand = 12;
	// number of bands 频段

	public float gThresh = 0.1f;
	// sensitivity灵敏度

//	int blipDelayLen = 16;
//	int[] blipDelay;

	private int sinceLast = 0;
	// counter to suppress double-beats到上一个beat之间的时间

	private float framePeriod;

	/* storage space */
	private int colmax = 120;//存当前帧之前的帧数

	float[] spectrum;//当前帧所有数据
	float[] averages;//当前帧 各频段 增加值 平均数
//	float[] acVals;
//	float[] onsets;
	float[] scorefun;//当前帧及之前120帧
//	float[] dobeat;
	int now = 0;
	// time index for circular buffer within above

	float[] spec;
	// the spectrum of the previous step

	/* Autocorrelation structure */
	int maxlag = 100;//最大间隔
	// (in frames) largest lag to track
	float decay = 0.997f;//衰减?
	// smoothing constant for running average
	Autoco auco;

	private float alph;//灵敏度相关？
	// trade-off constant between tempo deviation penalty and onset strength

	[Header ("Events")]
	public OnBeatEventHandler onBeat;
	public OnSpectrumEventHandler onSpectrum;

	//////////////////////////////////
	private long getCurrentTimeMillis ()
	{
		long milliseconds = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
		return milliseconds;
	}

	private void initArrays ()
	{
//		blipDelay = new int[blipDelayLen];
//		onsets = new float[colmax];//colmax = 120;//存当前帧之前的帧数
		scorefun = new float[colmax];
//		dobeat = new float[colmax];
		spectrum = new float[bufferSize];
		averages = new float[12];
//		acVals = new float[maxlag];//100最大间隔
		alph = 100 * gThresh;
	}

	// Use this for initialization
	void Start ()
	{
		initArrays ();

		audioSource = GetComponent<AudioSource> ();
		samplingRate = audioSource.clip.frequency;

		framePeriod = (float)bufferSize / (float)samplingRate;

		//initialize record of previous spectrum
		spec = new float[nBand];
		for (int i = 0; i < nBand; ++i)
			spec [i] = 100.0f;

		auco = new Autoco (maxlag, decay, framePeriod, getBandWidth ());

		lastT = getCurrentTimeMillis ();//上一帧时间
	}



	// Update is called once per frame
	void Update ()
	{
		if (audioSource.isPlaying) {
			audioSource.GetSpectrumData (spectrum, 0, FFTWindow.BlackmanHarris);//BlackmanHarris
			computeAverages (spectrum);
		//	onSpectrum.Invoke (averages);

			/* calculate the value of the onset function in this frame */
			float onset = 0;
			for (int i = 0; i < nBand; i++) {
				//12个频段
				float specVal = (float)System.Math.Max (-100.0f, 20.0f * (float)System.Math.Log10 (averages [i]) + 160); // dB value of this band
				specVal *= 0.025f;
				float dbInc = specVal - spec [i]; // dB increment since last frame当前频段,当前帧与前一帧增加值
				spec [i] = specVal; // record this frome to use next time around更新前一帧音量
				onset += dbInc; // onset function is the sum of dB increments所有频段的增加值
			}

//			onsets [now] = onset;//当前帧所有频段的增加值
			//onsets = new float[colmax];colmax = 120;存当前帧之前的帧数,now为在120帧内的当前时间,循环使用

			/* update autocorrelator and 找峰值间隔 find peak lag = current tempo */
			auco.newVal (onset);//保存100个增加值,处理数据

			// record largest value in (weighted) autocorrelation as it will be the tempo
			float aMax = 0.0f;
			int tempopd = 0;
			//float[] acVals = new float[maxlag];
			for (int i = 0; i < maxlag; ++i) {
				//在保存的100个数据中找最大值,平方或绝对值
				float acVal =(float)System.Math.Sqrt (auco.autoco (i));
				if (acVal > aMax) {
					aMax = acVal;
					tempopd = i;//记录最大值在100个中的位置
				}
				// store in array backwards, so it displays right-to-left, in line with traces
//				acVals [maxlag - 1 - i] = acVal;//acVals没有什么用?
				//倒序
			}



			//计算 scorefun ,
			/* calculate DP-ish function to update the best-score function */
			float smax = -999999;
			int smaxix = 0;
			// weight can be varied dynamically with the mouse
			alph = 100 * gThresh;
			// consider all possible preceding beat times from 0.5 to 2.0 x current tempo period
			for (int i = tempopd / 2; i < System.Math.Min (colmax, 2 * tempopd); ++i) {
				// objective function - this beat's cost + score to last beat + transition penalty
				float score = onset + scorefun [(now - i + colmax) % colmax] - alph * (float)System.Math.Pow (System.Math.Log ((float)i / (float)tempopd), 2);
				//onset当前增加值.now在120帧中位置,
				// keep track of the best-scoring predecesor
				if (score > smax) {
					smax = score;
					smaxix = i;
				}
			}

			scorefun [now] = smax;//当前帧的什么最大值?

			// keep the smallest value in the score fn window as zero, by subtracing the min val
			float smin = scorefun [0];
			for (int i = 0; i < colmax; ++i){
				if (scorefun [i] < smin){
					smin = scorefun [i];
					}
				}
			for (int i = 0; i < colmax; ++i){
				scorefun [i] -= smin;
				}
			//end计算 scorefun ,
 
				/////////////////////////////////////////////888

		//	scorefun [now] =onset;
				/////////////////////////////////////////////888


			/* find the largest value in the score fn window, to decide if we emit a blip */
			smax = scorefun [0];
			smaxix = 0;
			for (int i = 0; i < colmax; ++i) {
				if (scorefun [i] > smax) {
					smax = scorefun [i];
					smaxix = i;
				}
			}

			// dobeat array records where we actally place beats
//			dobeat [now] = 0;  // default is no beat this frame
			++sinceLast;
			// if current value is largest in the array, probably means we're on a beat 当前帧在120帧中score最大
			if (smaxix == now) {
				//tapTempo();
				// make sure the most recent beat wasn't too recently
				if (sinceLast > tempopd / 4) {
					onBeat.Invoke ();			
//					blipDelay [0] = 1;
					// record that we did actually mark a beat this frame
//					dobeat [now] = 1;
					// reset counter of frames since last beat
					sinceLast = 0;
				}
			}

			/* update column index (for ring buffer) */
			if (++now == colmax)
				now = 0;

		}
	}



	public float getBandWidth ()
	{
		//spec中有效的部分
		return (float)samplingRate/bufferSize;// (2f / (float)bufferSize) * (samplingRate / 2f);
	}

	public int freqToIndex (int freq)
	{
		// special case: freq is lower than the bandwidth of spectrum[0]
		if (freq < getBandWidth () / 2)
			return 0;
		// special case: freq is within the bandwidth of spectrum[512]
		if (freq > samplingRate / 2 - getBandWidth () / 2)
			return (bufferSize / 2);
		// all other cases
		float fraction = (float)freq / (float)samplingRate;
		int i = (int)System.Math.Round (bufferSize * fraction);
		//Debug.Log("frequency: " + freq + ", index: " + i);
		return i;
	}

	public void computeAverages (float[] data)
	{
		for (int i = 0; i < 12; i++) {
			//分12个频段,平均分配的,建议用对数分配?
			float avg = 0;
			int lowFreq;
			if (i == 0){
				lowFreq = 0;
			}else{
				lowFreq = (int)((samplingRate / 2) / (float)System.Math.Pow (2, 12 - i));
			}
			int hiFreq = (int)((samplingRate / 2) / (float)System.Math.Pow (2, 11 - i));


			int lowBound = freqToIndex (lowFreq);
			int hiBound = freqToIndex (hiFreq);
			for (int j = lowBound; j <= hiBound; j++) {
				//Debug.Log("lowbound: " + lowBound + ", highbound: " + hiBound);
				avg += data [j];
			}
			// line has been changed since discussion in the comments
			// avg /= (hiBound - lowBound);
			avg /= (hiBound - lowBound + 1);
			averages [i] = avg;//分12个频段保存平均值
		}
	}


	[System.Serializable]
	public class OnBeatEventHandler : UnityEngine.Events.UnityEvent
	{

	}

	[System.Serializable]
	public class OnSpectrumEventHandler : UnityEngine.Events.UnityEvent<float []>
	{

	}

	// class to compute an array of online autocorrelators
	private class Autoco
	{
		private int del_length;
		private float decay;
		private float[] delays;
		private float[] outputs;
		private int indx;

		private float[] bpms;
		private float[] rweight;
		private float wmidbpm = 120f;
		private float woctavewidth;

		//auco = new Autoco (maxlag, decay, framePeriod, getBandWidth ());100最大间隔,衰减,,spec中有效部分
		public Autoco (int len, float alpha, float framePeriod, float bandwidth)
		{
			woctavewidth = bandwidth;
			decay = alpha;//decay=0.997衰减?
			del_length = len;//100最大间隔
			delays = new float[del_length];
			outputs = new float[del_length];
			indx = 0;

			// calculate a log-lag gaussian weighting function, to prefer tempi around 120 bpm
			bpms = new float[del_length];
			rweight = new float[del_length];
			for (int i = 0; i < del_length; ++i) {
				bpms [i] = 60.0f / (framePeriod * (float)i);
				//Debug.Log(bpms[i]);
				// weighting is Gaussian on log-BPM axis, centered at wmidbpm, SD = woctavewidth octaves
				rweight [i] = (float)System.Math.Exp (-0.5f * System.Math.Pow (System.Math.Log (bpms [i] / wmidbpm) / System.Math.Log (2.0f) / woctavewidth, 2.0f));
				Debug.Log(rweight[i]);
			}
		}

		public void newVal (float val)
		{

			delays [indx] = val;//更新为前100帧

			// update running autocorrelator values
			for (int i = 0; i < del_length; ++i) {
				int delix = (indx - i + del_length) % del_length;//index前一帧在100帧中的位置
				outputs [i] += (1 - decay) * (delays [indx] * delays [delix] - outputs [i]);
				//outputs [i] =(1-衰减)*(当前-前帧)+outputs [i] *衰减
				//变化平滑
			}

			if (++indx == del_length)
				indx = 0;
		}

		// read back the current autocorrelator value at a particular lag
		public float autoco (int del)
		{
			float blah = rweight [del] * outputs [del];
			return blah;
		}


	}
}
