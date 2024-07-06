using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.IO;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь", typeof(string))]
    [NodeInput("текст", typeof(string))]
    [NodeInput("перезапись", typeof(bool))]

    public class TextRewrite : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
        {
            string link = (string)inputs[0].Value;
            string text = (string)inputs[1].Value;
            bool perezapis = (bool)inputs[2].Value;
            if (File.Exists(link))
            {
                if (perezapis == true)
                {
                    File.WriteAllText(link, text);
                }
                else
                {
                    string existingText = File.ReadAllText(link);
                    string newText = existingText + text.Replace("^Z", Environment.NewLine);
                    File.WriteAllText(link, newText);
                }
            }
            else
            {
                return new NodeResult("Файл не существует.");
            }

            return new NodeResult(link);
        }
    }
}