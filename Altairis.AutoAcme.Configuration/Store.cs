﻿using System;
using System.Collections.Generic;
using System.IO;
using Certes.Acme;
using Newtonsoft.Json;

namespace Altairis.AutoAcme.Configuration {
    public class Store {
        private string json;

        public string EmailAddress { get; set; } = "example@example.com";

        public string ChallengeFolder { get; set; } = @"C:\InetPub\wwwroot\AutoAcme";

        public string PfxFolder { get; set; } = @"C:\CertStore\PFX";

        public string PfxPassword { get; set; }

        public string PemFolder { get; set; }

        public Uri ServerUri { get; set; } = WellKnownServers.LetsEncrypt;

        public int ChallengeVerificationRetryCount { get; set; } = 10;

        public int ChallengeVerificationWaitSeconds { get; set; } = 5;

        public int RenewDaysBeforeExpiration { get; set; } = 30;

        public int PurgeDaysAfterExpiration { get; set; } = 30;

        public bool AutoSaveConfigBackup { get; set; } = true;

        public IList<Host> Hosts { get; set; } = new List<Host>();

        // Methods

        public void Save(string fileName, bool saveWhenNotChanged = false) {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(fileName));

            var newJson = JsonConvert.SerializeObject(this, Formatting.Indented);
            if (!saveWhenNotChanged && newJson.Equals(this.json, StringComparison.Ordinal)) return;

            // Save old version of configuration file -- on best effort basis
            if (this.AutoSaveConfigBackup && File.Exists(fileName)) {
                var oldFileName = fileName + ".old";
                try {
                    File.Copy(fileName, oldFileName, overwrite: true);
                }
#pragma warning disable RECS0022 // A catch clause that catches System.Exception and has an empty body
                catch (Exception) { }
#pragma warning restore RECS0022 // A catch clause that catches System.Exception and has an empty body
            }

            // Save new version
            File.WriteAllText(fileName, newJson);
            this.json = newJson;
        }

        public static Store Load(string fileName) {
            if (fileName == null) throw new ArgumentNullException(nameof(fileName));
            if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("Value cannot be empty or whitespace only string.", nameof(fileName));

            var jsonFromFile = File.ReadAllText(fileName);
            var store = JsonConvert.DeserializeObject<Store>(jsonFromFile);
            store.json = jsonFromFile;
            return store;
        }

    }
}
