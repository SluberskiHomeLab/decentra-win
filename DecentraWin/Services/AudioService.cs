using NAudio.Wave;

namespace DecentraWin.Services;

public class AudioService
{
    private WaveInEvent? _waveIn;
    private WaveOutEvent? _waveOut;
    private BufferedWaveProvider? _waveProvider;
    private bool _isMuted;

    public event EventHandler<byte[]>? AudioCaptured;
    public bool IsMuted => _isMuted;

    public void StartCapture()
    {
        try
        {
            _waveIn = new WaveInEvent
            {
                WaveFormat = new WaveFormat(48000, 16, 1)
            };

            _waveIn.DataAvailable += OnDataAvailable;
            _waveIn.StartRecording();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting audio capture: {ex.Message}");
        }
    }

    public void StopCapture()
    {
        if (_waveIn != null)
        {
            _waveIn.StopRecording();
            _waveIn.Dispose();
            _waveIn = null;
        }
    }

    public void StartPlayback()
    {
        try
        {
            _waveOut = new WaveOutEvent();
            _waveProvider = new BufferedWaveProvider(new WaveFormat(48000, 16, 1))
            {
                BufferDuration = TimeSpan.FromSeconds(2)
            };

            _waveOut.Init(_waveProvider);
            _waveOut.Play();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error starting audio playback: {ex.Message}");
        }
    }

    public void StopPlayback()
    {
        if (_waveOut != null)
        {
            _waveOut.Stop();
            _waveOut.Dispose();
            _waveOut = null;
        }
    }

    public void PlayAudio(byte[] audioData)
    {
        if (_waveProvider != null && !_isMuted)
        {
            _waveProvider.AddSamples(audioData, 0, audioData.Length);
        }
    }

    public void SetMuted(bool muted)
    {
        _isMuted = muted;
    }

    private void OnDataAvailable(object? sender, WaveInEventArgs e)
    {
        if (!_isMuted && e.BytesRecorded > 0)
        {
            var buffer = new byte[e.BytesRecorded];
            Buffer.BlockCopy(e.Buffer, 0, buffer, 0, e.BytesRecorded);
            AudioCaptured?.Invoke(this, buffer);
        }
    }
}
