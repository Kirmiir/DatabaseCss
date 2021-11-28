using System;
using System.Collections.Concurrent;
using DatabaseCss.DAL;
using Microsoft.Extensions.DependencyInjection;

namespace DatabaseCss
{
    public class DataBaseWatcher
    {
        private SqlDependencyEx _listener;
        private readonly ConcurrentBag<Action> _callbacks;

        private static DataBaseWatcher _instance;
        
        public static DataBaseWatcher GetInstance()
        {
            return _instance ??= new DataBaseWatcher();
        }

        private DataBaseWatcher()
        {
            _callbacks = new ConcurrentBag<Action>();
        }

        public void RegisterCallback(Action callback)
        {
            _callbacks.Add(callback);
        }
        
        public void Initialization(string connectionString)
        {
            _listener = new SqlDependencyEx(connectionString, "DatabaseCss", "CssFiles");
            _listener.TableChanged += (_, _) =>
            {
                foreach (var callback in _callbacks)
                {
                    callback();
                }
            };
            _listener.Start();
        }

        public void Termination()
        {
            _listener.Stop();
        }
    }
}