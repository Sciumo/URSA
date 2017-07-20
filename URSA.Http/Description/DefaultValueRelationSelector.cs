﻿using System;
using System.Linq;
using System.Reflection;
using URSA.Web.Http;
using URSA.Web.Http.Reflection;
using URSA.Web.Mapping;

namespace URSA.Web.Description.Http
{
    /// <summary>Provides a default source or target of a parameter.</summary>
    public class DefaultValueRelationSelector : IDefaultValueRelationSelector
    {
        private static readonly string[] PopularIdentifierPropertyNames = { "Id", "Identifier", "Identity", "Key" };

        /// <inheritdoc />
        public ParameterSourceAttribute ProvideDefault(ParameterInfo parameter, Verb verb)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return GetDefaultParameterSource(parameter);
        }

        /// <inheritdoc />
        public ResultTargetAttribute ProvideDefault(ParameterInfo parameter)
        {
            if (parameter == null)
            {
                throw new ArgumentNullException("parameter");
            }

            return GetDefaultResultTarget(parameter);
        }

        private static ParameterSourceAttribute GetDefaultParameterSource(ParameterInfo parameter)
        {
            if ((typeof(Guid) == parameter.ParameterType) || (typeof(DateTime) == parameter.ParameterType) ||
                (typeof(Guid).MakeByRefType() == parameter.ParameterType) || (typeof(DateTime).MakeByRefType() == parameter.ParameterType) ||
                ((parameter.ParameterType.IsIdentity()) && (PopularIdentifierPropertyNames.Contains(parameter.Name, StringComparer.OrdinalIgnoreCase))))
            {
                return FromUrlAttribute.For(parameter);
            }

            if ((!parameter.ParameterType.GetTypeInfo().IsValueType) && (typeof(string) != parameter.ParameterType) &&
                (!((parameter.ParameterType.GetTypeInfo().IsEnumerable()) && (parameter.ParameterType.GetTypeInfo().GetItemType().IsNumber()))))
            {
                return FromBodyAttribute.For(parameter);
            }

            return FromQueryStringAttribute.For(parameter);
        }

        private static ResultTargetAttribute GetDefaultResultTarget(ParameterInfo parameter)
        {
            var parameterType = parameter.ParameterType.UnwrapIfTask();
            if ((typeof(Guid) == parameter.ParameterType) || (typeof(DateTime) == parameter.ParameterType) ||
                (typeof(Guid).MakeByRefType() == parameterType) || (typeof(DateTime).MakeByRefType() == parameterType) ||
                ((parameterType.IsIdentity()) && (PopularIdentifierPropertyNames.Contains(parameter.Name, StringComparer.OrdinalIgnoreCase))))
            {
                return new ToHeaderAttribute(Header.Location);
            }

            if (parameter.Member is MethodInfo)
            {
                var methodInfo = (MethodInfo)parameter.Member;
                var declaringTypeInfo = methodInfo.DeclaringType.GetTypeInfo();
                Type implementation;
                if (((implementation = declaringTypeInfo.GetImplementationOfAny(typeof(IController<>), typeof(IAsyncController<>))) != null) &&
                    (declaringTypeInfo.GetRuntimeInterfaceMap(implementation).TargetMethods[0].Equals(methodInfo)) && (parameter.Position == 0))
                {
                    return new ToHeaderAttribute(Header.ContentRange);
                }
            }

            return new ToBodyAttribute();
        }
    }
}