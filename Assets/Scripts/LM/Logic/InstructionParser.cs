using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class InstructionParser
{
    public List<string> ParseAction(string cmd) 
    {
        List<string> cmdList = new();
        Regex regexAction = new(@"<action>(.*?)</action>");
        Match matchAction = regexAction.Match(cmd);

        while (matchAction.Success) {
            cmd = cmd.Replace(matchAction.Value, "");
            string nowCmd = matchAction.Groups[1].Value.Trim();

            cmdList.Add(nowCmd);

            matchAction = regexAction.Match(cmd);
        }

        return cmdList;
    }

    public string ParseStatus(string cmd)
    {
        List<string> cmdList = new();
        Regex regexStatus = new(@"<changestatus>(.*?)</changestatus>");
        Match matchStatus = regexStatus.Match(cmd);
        while (matchStatus.Success) {
            cmd = cmd.Replace(matchStatus.Value, "");
            string nowCmd = matchStatus.Groups[1].Value.Trim();

            cmdList.Add(nowCmd);

            matchStatus = regexStatus.Match(cmd);
        }
        if (cmdList.Count != 0) return cmdList[^1];
        return null;
    }

    public string ParseMessagePool(string cmd)
    {
        Regex regexStatus = new(@"<messagepool>(.*?)</messagepool>", RegexOptions.Singleline);
        Match matchStatus = regexStatus.Match(cmd);
        if (matchStatus.Success) {
            return matchStatus.Groups[1].Value.Trim();
        }
        return null;
    }

    public string ParseReflection(string cmd)
    {
        Regex regexStatus = new(@"<reflection>(.*?)</reflection>", RegexOptions.Singleline);
        Match matchStatus = regexStatus.Match(cmd);
        if (matchStatus.Success) {
            string matchedString = matchStatus.Groups[1].Value.Trim();
            try {
                ReflectionJson reflectionJson = JsonUtility.FromJson<ReflectionJson>(matchedString);
                return reflectionJson.reflection;
            }
            catch (Exception ex) {
                Debug.Log(ex);
                return null;
            }
        }
        return null;
    }

    public ManagerInfoJson ParseManagerAction(string cmd)
    {
        Regex regexStatus = new(@"<manager>(.*?)</manager>", RegexOptions.Singleline);
        Match matchStatus = regexStatus.Match(cmd);
        if (matchStatus.Success) {
            string matchedString = matchStatus.Groups[1].Value.Trim();
            try {
                ManagerInfoJson managerInfoJson = JsonUtility.FromJson<ManagerInfoJson>(matchedString);
                return managerInfoJson;
            }
            catch (Exception ex) {
                Debug.Log(ex);
                return null;
            }
        }
        return null;
    }

    public List<string> ParseMessage(string cmd)
    {
        List<string> cmdList = new();
        Regex regexStatus = new(@"<message>(.*?)</message>", RegexOptions.Singleline);
        Match matchStatus = regexStatus.Match(cmd);

        while (matchStatus.Success) {
            cmd = cmd.Replace(matchStatus.Value, "");
            string nowCmd = matchStatus.Groups[1].Value.Trim();

            cmdList.Add(nowCmd);

            matchStatus = regexStatus.Match(cmd);
        }

        return cmdList;
    }

    public string ParseOrder(string cmd)
    {
        Regex regexStatus = new(@"<order>(.*?)</order>");
        Match matchStatus = regexStatus.Match(cmd);
        if (matchStatus.Success) {
            return matchStatus.Groups[1].Value.Trim();
        }
        return null;
    }

    public string ParseSuggestion(string cmd)
    {
        Regex regexStatus = new(@"<suggestion>(.*?)</suggestion>");
        Match matchStatus = regexStatus.Match(cmd);
        if (matchStatus.Success) {
            return matchStatus.Groups[1].Value.Trim();
        }
        return null;
    }
}
