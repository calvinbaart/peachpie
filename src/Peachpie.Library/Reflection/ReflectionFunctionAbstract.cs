﻿using Pchp.Core;
using Pchp.Core.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Pchp.Library.Reflection
{
    [PhpType(PhpTypeAttribute.InheritName), PhpExtension(ReflectionUtils.ExtensionName)]
    public abstract class ReflectionFunctionAbstract : Reflector
    {
        #region Fields & Properties

        /// <summary>
        /// Gets name of the function.
        /// </summary>
        public string name
        {
            get
            {
                return _routine.Name;
            }
            //set
            //{
            //    // Read-only, throws ReflectionException in attempt to write.
            //    throw new ReflectionException(); // TODO: message
            //}
        }

        /// <summary>
        /// Underlaying routine information.
        /// Cannot be <c>null</c>.
        /// </summary>
        internal protected RoutineInfo _routine;

        #endregion

        //private void __clone(void ) { throw new NotImplementedException(); }
        public virtual ReflectionClass getClosureScopeClass() { throw new NotImplementedException(); }
        public virtual object getClosureThis() { throw new NotImplementedException(); }
        [return: CastToFalse]
        public string getDocComment() => null;
        public long getEndLine() { throw new NotImplementedException(); }
        public ReflectionExtension getExtension()
        {
            var extname = getExtensionName();
            return extname != null ? new ReflectionExtension(extname) : null;
        }
        [return: CastToFalse]
        public string getExtensionName()
        {
            var containingType = _routine.Methods[0].DeclaringType;
            var extensions = containingType.GetCustomAttribute<PhpExtensionAttribute>(false)?.Extensions;

            if (extensions != null && extensions.Length != 0)
            {
                return extensions[0];
            }

            return null;
        }
        public virtual string getFileName(Context ctx) { throw new NotImplementedException(); }
        public string getName() => name;
        public string getNamespaceName()
        {
            // opposite of getShortName()
            var name = this.name;
            var sep = name.LastIndexOf(ReflectionUtils.NameSeparator);
            return (sep < 0) ? string.Empty : name.Remove(sep);
        }
        public long getNumberOfParameters() { throw new NotImplementedException(); }
        public long getNumberOfRequiredParameters() { throw new NotImplementedException(); }

        /// <summary>
        /// Get the parameters as an array of <see cref="ReflectionParameter"/>.
        /// </summary>
        /// <returns>The parameters, as <see cref="ReflectionParameter"/> objects.</returns>
        public PhpArray getParameters()
        {
            var parameters = ReflectionUtils.ResolveReflectionParameters(this, _routine.Methods);

            //

            var arr = new PhpArray(parameters.Count);
            for (int i = 0; i < parameters.Count; i++)
            {
                arr.Add(PhpValue.FromClass(parameters[i]));
            }

            return arr;
        }
        public virtual ReflectionType getReturnType()
        {
            var returnType = _routine.Methods.Select(m => m.GetCustomAttribute<ReturnTypeAttribute>()).First();

            if (returnType == null)
            {
                return null;
            }

            return new ReflectionType()
            {
                Name = returnType.SyntaxType,
                AllowsNull = returnType.AllowsNull
            };
        }

        public string getShortName()
        {
            var name = this.name;
            var sep = name.LastIndexOf(ReflectionUtils.NameSeparator);
            return (sep < 0) ? name : name.Substring(sep + 1);
        }
        public long getStartLine() { throw new NotImplementedException(); }
        public PhpArray getStaticVariables() { throw new NotImplementedException(); }
        public bool hasReturnType() => _routine.Methods.Any(m => m.GetCustomAttribute<ReturnTypeAttribute>() != null);
        public bool inNamespace() => name.IndexOf(ReflectionUtils.NameSeparator) > 0;
        public virtual bool isClosure() { throw new NotImplementedException(); }
        public virtual bool isDeprecated() { throw new NotImplementedException(); }
        public bool isGenerator() { throw new NotImplementedException(); }
        public bool isInternal() => !isUserDefined();
        public bool isUserDefined() => _routine.IsUserFunction;
        public bool isVariadic() => _routine.Methods.Any(m => m.GetParameters().Any(p => p.GetCustomAttribute<ParamArrayAttribute>() != null));
        public bool returnsReference() => _routine.Methods.Any(m => m.ReturnType == typeof(PhpAlias));

        public virtual string __toString() { throw new NotImplementedException(); }

        public override string ToString() => __toString();
    }
}
