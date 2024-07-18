using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using ServiceViewer.Models;

namespace ServiceViewer.ViewModels;

public class MainViewModel : INotifyPropertyChanged
{
    private ObservableCollection<ServiceOffer> services;
    private ISdListener sdListener;
    private ISdMessageDeserializer sdMessageDeserializer;
    private string startStopButtonContent = "Start";
    private Object servicesLock = new Object();
    private CancellationTokenSource cancellationTokenSourceCleanup = new CancellationTokenSource();

    public event PropertyChangedEventHandler? PropertyChanged;

    public string SdIpAddress { get; set; } = "224.224.224.245";

    public string SdPort { get; set; } = "30490";

    public ObservableCollection<ServiceOffer> Services
    {
        get { return services; }
    }

    private bool isBusy;

    public bool IsBusy { get => this.isBusy; set
        {
            this.isBusy = value;
            OnPropertyChanged(nameof(IsBusy));
        }
    }

    public string StartStopButtonContent
    {
        get => this.startStopButtonContent;
        set
        {
            this.startStopButtonContent = value;
            OnPropertyChanged(nameof(StartStopButtonContent));
        }
    }

    // public ReactiveCommand<Unit, Unit> StartStopCommand { get; }

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public MainViewModel()
    {
        this.services = new ObservableCollection<ServiceOffer>();
        this.sdMessageDeserializer = new SdMessageDeserializer();

        this.sdListener = new SdListener(System.Net.IPAddress.Parse(this.SdIpAddress), int.Parse(this.SdPort));
        this.sdListener.DataReceived += this.OnDataReceived;
        this.sdListener.StateChanged += this.OnStateChanged;
    }

    private void OnStateChanged(object? sender, StateChangedEventArgs? e)
    {
        if (e == null)
        {
            return;
        }
        this.isBusy = false;
        this.StartStopButtonContent = e.IsRunning ? "Stop" : "Start";
    }

    private void CleanUpTaskAsync()
    {
        CancellationToken cancellationToken = this.cancellationTokenSourceCleanup.Token;
        while (!cancellationToken.IsCancellationRequested)
        {
            CleanUpServicesAsync();
            Thread.Sleep(50);
        }
    }

    private void CleanUpServicesAsync()
    {
        lock (servicesLock)
        {
            ulong now = (ulong)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;

            var task = Dispatcher.UIThread.InvokeAsync(() =>
            {
                this.Services.Where(s => (now - s.ReceptionTime) > (s.Ttl * 1000)).ToList().ForEach(s => this.Services.Remove(s));
            });
            task.Wait();
        }
    }

    private void OnDataReceived(object? sender, DataReceivedEventArgs? e)
    {
        if (e == null)
        {
            return;
        }

        try
        {
            SdMessage message = this.sdMessageDeserializer.Deserialize(e.Data);
            IEnumerable<ServiceOffer> offeredServices = ServiceOffer.FromSdMessage(message);

            foreach (var service in offeredServices)
            {
                if (this.Services.Any(s => s.Equals(service)))
                {
                    lock (servicesLock)
                    {
                        // Service already exists, update timestamp
                        var existingService = this.Services.First(s => s.Equals(service));
                        existingService.ReceptionTime = (ulong)DateTime.Now.ToUniversalTime().Subtract(new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
                        this.OnPropertyChanged(nameof(Services));
                    }
                    continue;
                }

                lock (servicesLock)
                {
                    var task = Dispatcher.UIThread.InvokeAsync(() => this.Services.Add(service));
                    task.Wait();
                }
            }
        }
        catch (Exception)
        {

        }
    }
    public void StartStopCommand()
    {
        this.isBusy = true;
        if (this.StartStopButtonContent == "Start")
        {
            this.sdListener.StartReceiving();
            this.cancellationTokenSourceCleanup = new CancellationTokenSource();
            Task.Run(() => this.CleanUpTaskAsync());
        }
        else
        {
            this.cancellationTokenSourceCleanup.Cancel();
            this.sdListener.StopReceiving();
        }
    }

    public bool CanStartStopCommand()
    {
        return false;
    }

}

