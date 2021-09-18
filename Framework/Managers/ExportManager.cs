using DryIoc;
using SDK.Server.Diagnostics;
using SDK.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Framework.Managers
{
    internal class ExportManager
    {
        private readonly IContainer _container;
        private const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance | BindingFlags.FlattenHierarchy;

        private Dictionary<string, Delegate> _exports = new Dictionary<string, Delegate>();

        public ExportManager(IContainer container)
        {
            _container = container;

            Logger.Write("ExportManager", "Initialized successfully");
        }

        public void CallMethod(string methodName, params object[] args)
        {
            try
            {
                if (_exports.ContainsKey(methodName))
                    _exports[methodName].DynamicInvoke(args);
                else
                    Logger.Debug($"Unable to call exported method: {methodName}, this export does not exists.");
            }
            catch (Exception ex)
            {
                Logger.Error($"Error on call exported method: {methodName}. Error: {ex.Message}");
            }
        }

        public T CallMethod<T>(string methodName, params object[] args)
        {
            try
            {
                if (_exports.ContainsKey(methodName))
                    return (T)_exports[methodName].DynamicInvoke(args);
                else
                    Logger.Debug($"Unable to call exported method: {methodName}, this export does not exists.");

                return (T)Activator.CreateInstance(typeof(T));
            }
            catch (Exception ex)
            {
                Logger.Error($"Error on call exported method: {methodName}. Error: {ex.Message}");
            }

            return (T)Activator.CreateInstance(typeof(T));
        }

        internal void Reflect()
        {
            var asm = Assembly.GetExecutingAssembly();
            var types = asm.GetTypes();

            foreach (var service in types)
            {
                if (_container.IsRegistered(service))
                {
                    // Continue if the service have the same type of this class
                    if (service == GetType()) continue;

                    // Get service instance
                    var _service = _container.GetService(service);
                    var methods = service.GetMethods(flags);

                    foreach (var method in methods)
                    {
                        var attr = method.GetCustomAttribute<ExportAttribute>();
                        if (attr == null) continue;

                        RegisterInternalExport(attr, _service, method);
                    }
                }
            }
        }

        internal void RegisterInternalExport(ExportAttribute exportAttr, object classObj, MethodInfo method)
        {
            var methodParams = method.GetParameters();

            try
            {
                if (!_exports.ContainsKey(exportAttr.Name))
                {
                    var action = Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
                    _exports.Add(exportAttr.Name, action);

                    Logger.Debug($"Registering [Export]: {exportAttr.Name} on method: {method.Name}.");
                }
                else
                {
                    Logger.Error($"Unable to register [Export]: {exportAttr.Name} on method: {method.Name}, an export have already been registered with this name.");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Export error: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}
