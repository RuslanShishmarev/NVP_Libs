﻿using NVP.API.Nodes;

using System;
using System.Collections.Generic;
using System.IO;

namespace NVP_Libs.Common
{
    [NodeInput("полный путь до файла", typeof(string))]
    [NodeInput("текст", typeof(string))]

    public class TextRedactor : INode
    {
        public NodeResult Execute(INVPData context, List<NodeResult> inputs)
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

            return new NodeResult(link);
        }
    }
}