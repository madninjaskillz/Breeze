using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Breeze.Helpers
{
    public static class ReflectionHelpers
    {
        public static bool DoesTypeSupportInterface(Type type, Type inter)
        {
            if (inter.IsAssignableFrom(type))
            {
                return true;
            }

            return type.GetInterfaces()
                .Any(i => i.GetTypeInfo().IsGenericType && i.GetGenericTypeDefinition() == inter);
        }

        public static IEnumerable<Type> TypesImplementingInterface(Type desiredType)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            List<Type> many = new List<Type>();
            foreach (Assembly assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes();
                    many.AddRange(types);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            var result = many.Where(type => DoesTypeSupportInterface(type, desiredType));

            return result;

            return AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(type => DoesTypeSupportInterface(type, desiredType));

        }


        public static void SetPropValue(this object car, string propName, object value)
        {
            var thing = car.GetType().GetProperties().Single(pi => pi.Name == propName);
            var thing2 = thing.GetSetMethod();
            thing2.Invoke(car, new object[] {value});


        }

        public static object GetPropertyValue(this object car, string propertyName)
        {
            var thing = car.GetType().GetProperties().Single(pi => pi.Name == propertyName);

            return thing;

            return car.GetType().GetProperties()
                .Single(pi => pi.Name == propertyName)
                .GetValue(car, null);
        }

        public static class MemberInfoGetting
        {
            public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
            {
                MemberExpression expressionBody = (MemberExpression) memberExpression.Body;
                return expressionBody.Member.Name;
            }
        }

        //public static IEnumerable<Type> FindDerivedTypesFromAssembly(this Assembly assembly, Type baseType,
        //    bool classOnly)
        //{
        //    if (assembly == null)
        //        throw new ArgumentNullException("assembly", "Assembly must be defined");

        //    if (baseType == null)
        //        throw new ArgumentNullException("baseType", "Parent Type must be defined");
        //    // get all the types
        //    var types = assembly.GetTypes();
        //    // works out the derived types
        //    foreach (var type in types)
        //    {
        //        // if classOnly, it must be a class
        //        // useful when you want to create instance
        //        if (classOnly && !type.IsClass)
        //            continue;
        //        if (baseType.IsInterface)
        //        {
        //            var it = type.GetInterface(baseType.FullName);
        //            if (it != null)
        //                // add it to result list
        //                yield return type;
        //        }
        //        else if (type.IsSubclassOf(baseType))
        //        {
        //            // add it to result list
        //            yield return type;
        //        }
        //    }
        //}

        //public static IEnumerable<Type> FindDerivedTypesFromAssembly(Type baseType, bool classOnly)
        //{
        //    var assemblies = AppDomain.CurrentDomain.GetAssemblies();

        //    if (baseType == null)
        //        throw new ArgumentNullException("baseType", "Parent Type must be defined");
        //    // get all the types
        //    foreach (Assembly assembly in assemblies)
        //    {

        //        var types = assembly.GetTypes();
        //        // works out the derived types
        //        foreach (var type in types)
        //        {
        //            // if classOnly, it must be a class
        //            // useful when you want to create instance
        //            if (classOnly && !IsClass)
        //                continue;
        //            if (baseType.IsInterface)
        //            {
        //                var it = type.GetInterface(baseType.FullName);
        //                if (it != null)
        //                    // add it to result list
        //                    yield return type;
        //            }
        //            else if (type.IsSubclassOf(baseType))
        //            {
        //                // add it to result list
        //                yield return type;
        //            }
        //        }
        //    }
        //}
    }

    public static class ClassExtensions{

//    public static ConstructorInfo GetDefaultConstructor(this Type type)
//        {
//#if NET45
//            var typeInfo = type.GetTypeInfo();
//            var ctor = typeInfo.DeclaredConstructors.FirstOrDefault(c => !c.IsStatic && c.GetParameters().Length == 0);
//            return ctor;
//#else
//            var attrs = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
//            return type.GetConstructor(attrs, null, new Type[0], null);
//#endif
//        }

        public static PropertyInfo[] GetAllProperties(this Type type)
        {

            // Sometimes, overridden properties of abstract classes can show up even with 
            // BindingFlags.DeclaredOnly is passed to GetProperties. Make sure that
            // all properties in this list are defined in this class by comparing
            // its get method with that of it's base class. If they're the same
            // Then it's an overridden property.
#if NET45
            PropertyInfo[] infos= type.GetTypeInfo().DeclaredProperties.ToArray();
            var nonStaticPropertyInfos = from p in infos
                                         where (p.GetMethod != null) && (!p.GetMethod.IsStatic) &&
                                         (p.GetMethod == p.GetMethod.GetRuntimeBaseDefinition())
                                         select p;
            return nonStaticPropertyInfos.ToArray();
#else
            const BindingFlags attrs = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            var allProps = type.GetProperties(attrs).ToList();
            var props = allProps.FindAll(p => p.GetGetMethod(true) != null && p.GetGetMethod(true) == p.GetGetMethod(true).GetBaseDefinition()).ToArray();
            return props;
#endif
        }


        public static FieldInfo[] GetAllFields(this Type type)
        {
#if NET45
            FieldInfo[] fields= type.GetTypeInfo().DeclaredFields.ToArray();
            var nonStaticFields = from field in fields
                    where !field.IsStatic
                    select field;
            return nonStaticFields.ToArray();
#else
            var attrs = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;
            return type.GetFields(attrs);
#endif
        }

        public static bool IsClass(this Type type)
        {
#if !NET45
            return type.GetTypeInfo().IsClass;
#else
            return type.IsClass;
#endif
        }

    }
}
