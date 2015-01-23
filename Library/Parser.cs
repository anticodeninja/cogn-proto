using System.Linq;
using System.Security.Cryptography;
using Jint;

namespace Library
{
    using System;
    using System.IO;
    using System.Threading;

    public class Parser
    {
        private readonly string _inputFile;
        private readonly FileSystemWatcher _watcher;
        private readonly Engine _engine;
        private byte[] _oldFileHash;

        public class ReloadingArgs : EventArgs
        {
             
        }

        public class ReloadedArgs : EventArgs
        {
            public ReloadedArgs(string message)
            {
                Message = message;
            }

            public string Message { get; private set; }
        }

        public event EventHandler<ReloadingArgs> Reloading;

        public event EventHandler<ReloadedArgs> Reloaded;

        public Parser(string inputFile)
        {
            _inputFile = inputFile;
            _engine = new Engine();
            _watcher = new FileSystemWatcher
            {
                Path = "./",
                NotifyFilter = NotifyFilters.LastWrite,
                Filter = string.Format("*{0}*", _inputFile)
            };
            _watcher.Changed += OnInputFileChanged;
        }

        public void Start()
        {
            _watcher.EnableRaisingEvents = true;
            ParseFile();
        }

        private void OnInputFileChanged(object sender, FileSystemEventArgs args)
        {
            ParseFile();
        }

        public void Stop()
        {
            _watcher.EnableRaisingEvents = false;
        }

        private void ParseFile()
        {
            lock (this)
            {
                try
                {
                    Thread.Sleep(500);

                    var fileHash = ComputeHash();
                    if (_oldFileHash != null && _oldFileHash.SequenceEqual(fileHash))
                    {
                        return;
                    }
                    _oldFileHash = fileHash;

                    OnReloading(new ReloadingArgs());
                    _engine.Execute(File.ReadAllText(_inputFile));
                    OnReloaded(new ReloadedArgs(null));
                }
                catch (Exception ex)
                {
                    OnReloaded(new ReloadedArgs(ex.Message));
                }
            }
        }

        private byte[] ComputeHash()
        {
            using (var md5 = MD5.Create())
            {
                return md5.ComputeHash(File.ReadAllBytes(_inputFile));
            }
        }

        public void Add(string term, Delegate action)
        {
            _engine.SetValue(term, action);
        }

        protected virtual void OnReloading(ReloadingArgs e)
        {
            var handler = Reloading;
            if (handler != null)
                handler(this, e);
        }

        protected virtual void OnReloaded(ReloadedArgs e)
        {
            var handler = Reloaded;
            if (handler != null)
                handler(this, e);
        }
    }
}
