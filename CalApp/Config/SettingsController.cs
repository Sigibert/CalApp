using System;
using CalApp.Preferences;

namespace CalApp.Config
{
    public sealed class SettingsController
    {
        private readonly SettingsStore store;
        private readonly IStartupRegistration startup;
        private readonly string executablePath;
        public event EventHandler Saved;

        public AppSettings Settings => store.Current;
        public string LoadWarning => store.LoadWarning;

        public SettingsController(SettingsStore store, IStartupRegistration startup, string executablePath)
        {
            this.store = store ?? throw new ArgumentNullException(nameof(store));
            this.startup = startup ?? throw new ArgumentNullException(nameof(startup));
            this.executablePath = executablePath ?? throw new ArgumentNullException(nameof(executablePath));
            store.Load();
        }

        public void Apply(AppSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));
            settings.Validate();
            string previousCommand = startup.Command;
            string desiredCommand = settings.StartWithWindows ? "\"" + executablePath + "\"" : null;
            bool changedStartup = previousCommand != desiredCommand;
            if (changedStartup) startup.Command = desiredCommand;
            try
            {
                store.Save(settings);
            }
            catch (Exception saveError)
            {
                if (changedStartup)
                {
                    try { startup.Command = previousCommand; }
                    catch (Exception restoreError)
                    {
                        throw new InvalidOperationException("Preferences could not be saved and the previous Windows startup setting could not be restored. Check both settings.",
                            new AggregateException(saveError, restoreError));
                    }
                }
                throw;
            }
            Saved?.Invoke(this, EventArgs.Empty);
        }
    }
}
