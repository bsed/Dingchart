using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace cn.lds.chatcore.pcw.Common.Utils {
public partial class BeanUtils {
    /// <summary>
    ///
    /// </summary>
    /// <param Name="source"></param>
    /// <param Name="destination"></param>
    public static void copyProperties(object destination, object source) {
        var count = 1;

        CopyPropertiesRecursive(source, destination, null, ref count);

        //Console.WriteLine();
        //Console.WriteLine("Counted: " + (count - 1));
        //Console.WriteLine();
    }

    private static void CopyPropertiesRecursive(object source, object destination,
            IList<String> propertiesToOmmit, ref int count) {
        var destinationType = destination.GetType();

        Type sourceType = null;

        if (source != null)
            sourceType = source.GetType();

        var destinationProperties = destinationType.GetProperties();

        //for a type coming from a serialized web service type
        if (propertiesToOmmit == null)
            propertiesToOmmit = new List<string> { "ExtensionData" };

        destinationProperties = Array.FindAll(destinationProperties, pi => !propertiesToOmmit.Contains(pi.Name));

        foreach (var property in destinationProperties) {
            var propertyType = property.PropertyType;

            //todo can cache this as: static readonly Dictionary<Type,object> cache. how about multithreading?
            var sourceValue = propertyType.IsValueType ? Activator.CreateInstance(propertyType) : null;

            PropertyInfo propertyInSource = null;
            var sourceHasDestinationProperty = false;

            //source is null
            if (source != null) {
                propertyInSource = sourceType.GetProperty(property.Name);

                //source has the property
                if (propertyInSource != null) {
                    sourceHasDestinationProperty = true;
                    sourceValue = propertyInSource.GetValue(source, null);
                }
                //else Console.WriteLine("\tsource does not contain property " + property.Name);
            }
            //else
            //  Console.WriteLine("\tsource was null for " + destinationType + " -> " + property.Name);


            //it's a complex/container type?
            var isComplex = !propertyType.ToString().StartsWith("System");

            if (isComplex & !propertyType.IsArray) {
                //Console.WriteLine("\tRecursion on: " + property.Name);

                //create new destination structure

                var ci = propertyType.GetConstructor(Type.EmptyTypes);
                var newDestination = ci.Invoke(null);

                property.SetValue(destination, newDestination, null);

                //Console.WriteLine("\tCalled constructor on " + property.Name);

                CopyPropertiesRecursive(sourceValue, newDestination, propertiesToOmmit, ref count);
                continue;
            }

            //var s = count + ". " + property.Name + (propertyType.IsArray ? "[]" : "") + " = ";

            //if (!sourceHasDestinationProperty)
            //    s += "[default(" + propertyType + ")] = ";

            //Console.WriteLine(s + sourceValue);


            //todo check for CanWrite and CanRead - if (!toField.CanWrite) continue;

            if (propertyType.IsArray & propertyInSource != null)
                sourceValue = DeepCopyArray(propertyInSource.PropertyType, propertyType, sourceValue, source, destination);
            try {
                property.SetValue(destination, sourceValue, null);
            } catch (Exception e) {
                Log.Error(typeof(BeanUtils), e);
            }

            var destinationValue = property.GetValue(destination, null);

            count++;

            //todo deep assert for arrays
            //if (!propertyType.IsArray)
            //    Assert.AreEqual(sourceValue, destinationValue, "Assert failed for property: " + destinationType + "." + property.Name);
        }
    }

    private static object DeepCopyArray(Type sourceType, Type destinationType, object sourceValue, object sourceParent, object destinationParent) {
        //todo this method ca be made generic and handle more than just arrays

        if (sourceValue == null || sourceType == null || sourceParent == null || destinationParent == null)
            return null;

        using (var stream = new MemoryStream(2 * 1024)) {
            var serializer = new DataContractSerializer(sourceType);

            serializer.WriteObject(stream, sourceValue);

            serializer = new DataContractSerializer(destinationType);


            //if we know the namesapce or type names will be different, we must get the xml and REPLACE the
            //source type Name/namespace to destination source type Name/namespace

            //if the namespace/type Name combination is the same, we do not need the if TRUE statement,
            //only the ELSE branch

            if (true) {
                var xml = Encoding.UTF8.GetString(stream.ToArray());

                /*
                 <ArrayOfV1KeyValuePair  xmlns="urn:mycompany.com/data/version_1.0" xmlns:i="http://www.w3.org/2001/XMLSchema-instance">
                    <V1KeyValuePair>
                        <Key>jet key 1</Key>
                        <Value>jet value 1</Value>
                    </V1KeyValuePair>
                    <V1KeyValuePair>
                        <Key>jet key 2</Key>
                        <Value>jet value 2</Value>
                    </V1KeyValuePair>
                 </ArrayOfV1KeyValuePair>
                */

                var nameSource = sourceType.Name.Replace("[]", "");
                var nameDestination = destinationType.Name.Replace("[]", "");

                xml = xml.Replace(nameSource, nameDestination);

                var sourceNamespace = GetDataContractNamespace(sourceParent);
                var destiantionNamespace = GetDataContractNamespace(destinationParent);

                xml = xml.Replace(sourceNamespace, destiantionNamespace);

                using (var modified = new MemoryStream(Encoding.UTF8.GetBytes(xml))) {
                    modified.Position = 0;

                    return serializer.ReadObject(modified);
                }
            } else {
                stream.Position = 0;

                return serializer.ReadObject(stream);
            }
        }
    }

    private static string GetDataContractNamespace(object instance) {
        if (instance == null)
            throw new ArgumentNullException("instance");

        var attribute = instance.GetType().GetCustomAttributes(true).Single(o => o.GetType() == typeof(DataContractAttribute));

        return ((DataContractAttribute)attribute).Namespace;
    }
}
}
