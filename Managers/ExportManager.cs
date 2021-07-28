﻿using SDK.Server.Diagnostics;
using SDK.Shared;
using SDK.Shared.Exports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Average.Managers
{
    public class ExportManager : IExportManager
    {
        Dictionary<string, Delegate> exports;
        Logger logger;

        public ExportManager(Logger logger)
        {
            this.logger = logger;
            exports = new Dictionary<string, Delegate>();
        }

        public void CallMethod(string methodName, params object[] args)
        {
            if (exports.ContainsKey(methodName))
            {
                exports[methodName].DynamicInvoke(args);
            }
            else
            {
                logger.Debug($"Unable to call export: {methodName}, this export does not exists.");
            }
        }

        public T CallMethod<T>(string methodName, params object[] args)
        {
            if (exports.ContainsKey(methodName))
            {
                return (T)exports[methodName].DynamicInvoke(args);
            }
            else
            {
                logger.Debug($"Unable to call export: {methodName}, this export does not exists.");
            }

            return (T)Activator.CreateInstance(typeof(T));
        }

        public void RegisterExport(MethodInfo method, ExportAttribute exportAttr, object classObj)
        {
            var methodParams = method.GetParameters();

            if (!exports.ContainsKey(exportAttr.Name))
            {
                var action = Action.CreateDelegate(Expression.GetDelegateType((from parameter in method.GetParameters() select parameter.ParameterType).Concat(new[] { method.ReturnType }).ToArray()), classObj, method);
                exports.Add(exportAttr.Name, action);

                logger.Debug($"Registering [Export] attribute: {exportAttr.Name} on method: {method.Name}, args count: {methodParams.Count()}.");
            }
            else
            {
                logger.Error($"Unable to register [Export] attribute: {exportAttr.Name} on method: {method.Name}, an export have already been registered with this name.");
            }
        }

        #region Internal

        internal void InternalCallMethod(string methodName, List<object> args)
        {
            CallMethod(methodName, args.ToArray());
        }

        #endregion
    }
}
