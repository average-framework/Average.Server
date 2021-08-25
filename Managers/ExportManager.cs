using SDK.Server.Diagnostics;
using SDK.Shared;
using SDK.Shared.Exports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Server.Managers
{
    public class ExportManager : IExportManager
    {
        private Dictionary<string, Delegate> _exports = new Dictionary<string, Delegate>();
        
        public void CallMethod(string methodName, params object[] args)
        {
            try
            {
                if (_exports.ContainsKey(methodName))
                    _exports[methodName].DynamicInvoke(args);
                else
                    Log.Debug($"Unable to call export: {methodName}, this export does not exists.");
            }
            catch (Exception ex)
            {
                Log.Error($"Error on call method: {methodName}. Error: {ex.Message}");
            }
        }

        public T CallMethod<T>(string methodName, params object[] args)
        {
            try
            {
                if (_exports.ContainsKey(methodName))
                    return (T)_exports[methodName].DynamicInvoke(args);
                else
                    Log.Debug($"Unable to call export: {methodName}, this export does not exists.");

                var instance = (T)Activator.CreateInstance(typeof(T));
                return instance;
            }
            catch (Exception ex)
            {
                Log.Error($"Error on call method: {methodName}. Error: {ex.Message}");
            }

            return (T)Activator.CreateInstance(typeof(T));
        }

        public void RegisterExport(MethodInfo method, ExportAttribute exportAttr, object classObj)
        {
            var methodParams = method.GetParameters();

            if (!_exports.ContainsKey(exportAttr.Name))
            {
                var action = Delegate.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
                _exports.Add(exportAttr.Name, action);

                Log.Debug($"Registering [Export] attribute: {exportAttr.Name} on method: {method.Name}, args count: {methodParams.Count()}.");
            }
            else
            {
                Log.Error($"Unable to register [Export] attribute: {exportAttr.Name} on method: {method.Name}, an export have already been registered with this name.");
            }
        }
    }
}
