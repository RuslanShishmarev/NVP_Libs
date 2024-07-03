using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.IO;



namespace NVP_Libs.Common
{
    [NodeInput("Полный путь", typeof(string))]
    [NodeInput("Текст", typeof(string))]
    [NodeInput("Перезапись", typeof(bool))]

    public class TextRewrite : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
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

            return new NodeResult("Файл успешно отредактирован.");
        }
    }
}