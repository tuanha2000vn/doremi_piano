public delegate void ShortMessageEventHandler(int aCommnad, int aData1, int aData2);
	
public interface IMidiEvents{
    event ShortMessageEventHandler ShortMessageEvent;			
}

public interface IMidiSequencer{
	void PlayPause();
	void Pause();
	void Stop();
}
	
public interface IFrameFeed {
	int SampleRate();
	int DeltaFrames();
		
	void PlayFeed();
	void StopFeed();
	void PauseFeed();
}

