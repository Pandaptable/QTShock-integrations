﻿using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Celeste.Mod.QTShock_Celeste;
public class QTShock_CelesteModule : EverestModule {
    public static QTShock_CelesteModule Instance { get; private set; }

    public override Type SettingsType => typeof(QTShock_CelesteModuleSettings);
    public static QTShock_CelesteModuleSettings Settings => (QTShock_CelesteModuleSettings) Instance._Settings;
    
    private static HttpClient client = new();
    private string qtShockIp;

    public QTShock_CelesteModule() {
        Instance = this;
#if DEBUG
        // debug builds use verbose logging
        Logger.SetLogLevel(nameof(QTShock_CelesteModule), LogLevel.Verbose);
#else
        // release builds use info logging to reduce spam in log files
        Logger.SetLogLevel(nameof(QTShock_CelesteModule), LogLevel.Info);
#endif
    }

    public override void Load() {
        Everest.Events.Player.OnDie += OnDie;
        qtShockIp = Dns.GetHostEntry("qtshock.local").AddressList[0].ToString();
    }

    public override void Unload() {
        // TODO: unapply any hooks applied in Load()
    }
    private void OnDie(Player p)
    {
        if (!Settings.Toggle) return;
        
        Dictionary<string, string> paramsDict = new Dictionary<string, string>();
        paramsDict.Add("shocker", Settings.Shocker.ToString());
        
        if (client.BaseAddress == null) {
            client.BaseAddress = new Uri("http://" + qtShockIp); 
        }
        switch (Settings.Type)
        {
            case QTShock_CelesteModuleSettings.EnumType.Beep:
                _ = Task.Run(async () =>
                {
                    var response = await client.PostAsync("/beep", new FormUrlEncodedContent(paramsDict)); 
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                });
                break;

            case QTShock_CelesteModuleSettings.EnumType.Vibrate:
                paramsDict.Add("strength", Settings.Strength.ToString());
                _ = Task.Run(async () =>
                {
                    var response = await client.PostAsync("/vibrate", new FormUrlEncodedContent(paramsDict)); 
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                });
                break;

            case QTShock_CelesteModuleSettings.EnumType.Shock:
                paramsDict.Add("strength", Settings.Strength.ToString());
                _ = Task.Run(async () =>
                {
                    var response = await client.PostAsync("/shock", new FormUrlEncodedContent(paramsDict)); 
                    Console.WriteLine(response.StatusCode);
                    Console.WriteLine(await response.Content.ReadAsStringAsync());
                });
                break;
        }
    }
}