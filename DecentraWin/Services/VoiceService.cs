using DecentraWin.Models;
using SIPSorcery.Net;
using System.Net;

namespace DecentraWin.Services;

public class VoiceService
{
    private readonly WebSocketService _webSocketService;
    private readonly AudioService _audioService;
    private readonly Dictionary<string, RTCPeerConnection> _peerConnections = new();
    private bool _inVoiceChannel;
    private string? _currentChannelId;

    public event EventHandler<string>? CallReceived;
    public bool InVoiceChannel => _inVoiceChannel;

    public VoiceService(WebSocketService webSocketService, AudioService audioService)
    {
        _webSocketService = webSocketService;
        _audioService = audioService;
        _audioService.AudioCaptured += OnAudioCaptured;
    }

    public async Task JoinVoiceChannelAsync(string serverId, string channelId)
    {
        try
        {
            _currentChannelId = channelId;
            _inVoiceChannel = true;

            await _webSocketService.SendMessageAsync(new WebSocketMessage
            {
                Type = "join_voice",
                ServerId = serverId,
                ChannelId = channelId
            });

            _audioService.StartCapture();
            _audioService.StartPlayback();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error joining voice channel: {ex.Message}");
        }
    }

    public async Task LeaveVoiceChannelAsync()
    {
        try
        {
            _inVoiceChannel = false;
            _currentChannelId = null;

            await _webSocketService.SendMessageAsync(new WebSocketMessage
            {
                Type = "leave_voice"
            });

            _audioService.StopCapture();
            _audioService.StopPlayback();

            foreach (var pc in _peerConnections.Values)
            {
                pc.Close("leaving channel");
            }
            _peerConnections.Clear();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error leaving voice channel: {ex.Message}");
        }
    }

    public async Task CallUserAsync(string targetUsername)
    {
        try
        {
            var pc = CreatePeerConnection(targetUsername);
            var offer = pc.createOffer();
            await pc.setLocalDescription(offer);

            await _webSocketService.SendMessageAsync(new WebSocketMessage
            {
                Type = "webrtc_offer",
                Target = targetUsername,
                Offer = offer
            });

            _audioService.StartCapture();
            _audioService.StartPlayback();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error calling user: {ex.Message}");
        }
    }

    public async Task HandleOfferAsync(string from, RTCSessionDescriptionInit offer)
    {
        try
        {
            var pc = CreatePeerConnection(from);
            pc.setRemoteDescription(offer);

            var answer = pc.createAnswer();
            await pc.setLocalDescription(answer);

            await _webSocketService.SendMessageAsync(new WebSocketMessage
            {
                Type = "webrtc_answer",
                Target = from,
                Answer = answer
            });

            _audioService.StartCapture();
            _audioService.StartPlayback();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling offer: {ex.Message}");
        }
    }

    public async Task HandleAnswerAsync(string from, RTCSessionDescriptionInit answer)
    {
        try
        {
            if (_peerConnections.TryGetValue(from, out var pc))
            {
                pc.setRemoteDescription(answer);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling answer: {ex.Message}");
        }
    }

    public async Task HandleIceCandidateAsync(string from, RTCIceCandidateInit candidate)
    {
        try
        {
            if (_peerConnections.TryGetValue(from, out var pc))
            {
                pc.addIceCandidate(candidate);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error handling ICE candidate: {ex.Message}");
        }
    }

    public void SetMuted(bool muted)
    {
        _audioService.SetMuted(muted);
    }

    private RTCPeerConnection CreatePeerConnection(string peerId)
    {
        var pc = new RTCPeerConnection();

        pc.onicecandidate += async (candidate) =>
        {
            if (candidate != null)
            {
                await _webSocketService.SendMessageAsync(new WebSocketMessage
                {
                    Type = "webrtc_ice",
                    Target = peerId,
                    Candidate = candidate
                });
            }
        };

        pc.onconnectionstatechange += (state) =>
        {
            Console.WriteLine($"Connection state changed to: {state}");
        };

        _peerConnections[peerId] = pc;
        return pc;
    }

    private void OnAudioCaptured(object? sender, byte[] audioData)
    {
        // Send audio data to all peer connections
        foreach (var pc in _peerConnections.Values)
        {
            // Audio data would be sent through data channels or media tracks
            // This is simplified - actual implementation would use media tracks
        }
    }
}
