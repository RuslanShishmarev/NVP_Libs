using NVP.API.Nodes;
using System;
using System.Collections.Generic;
using System.IO;

namespace NVP_Libs
{
    [NodeInput("Link", typeof(string))]
    [NodeInput("Text", typeof(string))]

    public class Text_redactor : IRevitNode
    {
        public NodeResult Execute(IVisualViewerData context, List<NodeResult> inputs, object commandData)
        {
            string link = (string)inputs[0].Value;
            string text = (string)inputs[1].Value;

            if (File.Exists(link))
            {
                string existingText = File.ReadAllText(link);
                string newText = existingText + text.Replace("^Z", Environment.NewLine);
                File.WriteAllText(link, newText);
            }
            else
            {
                return new NodeResult("Файл не существует.");
            }

            return new NodeResult("Файл успешно отредактирован.");
        }
    }
}