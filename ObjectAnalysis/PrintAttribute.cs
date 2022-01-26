
using System;
using System.Collections;
using System.Collections.Generic;
using Almond.Network;
using UnityEngine;

namespace DefaultNamespace
{
    public class PrintAttribute : MonoBehaviour
    {
        struct TestSTRUCT
        {
            public int[] twest;
        }

        private void Start()
        {
            var exp = new ST_DELETE_MAIL_RECV();
            exp.ucMailType = MailType.SystemMail;
            exp.ullMailKey = new List<ulong>();
            exp.packetList = new List<string>();
            
            exp.ullMailKey.Add(111);
            exp.ullMailKey.Add(122);
            exp.ullMailKey.Add(133);
            
            printValue(exp);
            
            TestSTRUCT aa = new TestSTRUCT();
            aa.twest = new int[10];

            // printValue(aa);
        }

        private bool IsStruct(Type t)
        {
            return (t.IsValueType && !t.IsEnum && !t.IsPrimitive);
        }
        
        private string printValue(object notValue, string str = "")
        {
            var t = notValue.GetType();
            if ((t.IsValueType && !IsStruct(t))||typeof(string).IsAssignableFrom(t)) // Value && Enum
            {
                Debug.Log($"{str}\t{t.Name} : \t{notValue}");
            }
            else if(t.IsArray)                                // Array
            {
                str += "\t";
                var i = 0;
                foreach (var item in (Array) notValue)
                {
                    printValue(item, $"{str}[{i}]");
                    i++;
                }
            }
            else if(typeof(IList).IsAssignableFrom(t))        // List
            {
                str += "\t";
                var i = 0;
                foreach (var item in (IList) notValue)
                {
                    printValue(item, $"{str}[{i}]");
                    i++;
                }
            }
            else if(typeof(IDictionary).IsAssignableFrom(t)) // Dictionary
            {
                str += "\t";
                var i = 0;
                foreach (var item in (IDictionary) notValue)
                {
                    printValue(item, $"{str}[{i}]");
                    i++;
                }
            }
            else if(typeof(Queue).IsAssignableFrom(t)) // Queue
            {
                str += "\t";
                var i = 0;
                foreach (var item in (Queue) notValue)
                {
                    printValue(item, $"{str}[{i}]");
                    i++;
                }
            }
            else if(typeof(Stack).IsAssignableFrom(t)) // Stack
            {
                str += "\t";
                var i = 0;
                foreach (var item in (Stack) notValue)
                {
                    printValue(item, $"{str}[{i}]");
                    i++;
                }
            }
            else if(IsStruct(t) || t.IsClass) // Struct || Class
            {
                str += "\t";
                var fieldInfos = t.GetFields();
                foreach (var field in fieldInfos)
                {
                    Debug.Log($"{str}______________________________________________\n"
                              +$"{str}Name : \t\t{field.Name}\n"
                              +$"{str}FieldType : \t{field.FieldType}\n"
                              +$"{str}IsClass : \t{field.FieldType.IsClass}\n"
                              +$"{str}IsValueType : \t{field.FieldType.IsValueType}\n"
                              +$"{str}IsEnum : \t{field.FieldType.IsEnum}\n"
                              +$"{str}IsPrimitive : \t{field.FieldType.IsPrimitive}\n"
                              +$"{str}----------------------------------------"
                              );
                    printValue(field.GetValue(notValue), str+field.Name);
                }
            }

            return str;
        }
    }
}