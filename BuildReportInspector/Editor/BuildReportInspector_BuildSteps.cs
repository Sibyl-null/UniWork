using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Unity.BuildReportInspector
{
    public partial class BuildReportInspector
    {
        private static readonly List<LogType>
            ErrorLogTypes = new() { LogType.Error, LogType.Assert, LogType.Exception };

        private readonly BuildStepNode _rootStepNode = new BuildStepNode(null, -1);
        
        private void OnBuildStepGUI()
        {
            if(!_rootStepNode.children.Any())
            {
                // re-create steps hierarchy
                var branch = new Stack<BuildStepNode>();
                branch.Push(_rootStepNode);
                foreach (var step in Report.steps)
                {
                    while (branch.Peek().depth >= step.depth)
                    {
                        branch.Pop();
                    }

                    while (branch.Peek().depth < step.depth - 1)
                    {
                        var intermediateNode = new BuildStepNode(null, branch.Count - 1);
                        branch.Peek().children.Add(intermediateNode);
                        branch.Push(intermediateNode);
                    }

                    var stepNode = new BuildStepNode(step, step.depth);
                    branch.Peek().children.Add(stepNode);
                    branch.Push(stepNode);
                }

                _rootStepNode.UpdateWorstChildrenLogType();

                // expand first step, usually "Build player"
                if (_rootStepNode.children.Any())
                    _rootStepNode.children[0].foldoutState = true;
            }

            bool odd = false;
            foreach(var stepNode in _rootStepNode.children)
                stepNode.LayoutGUI(ref odd, 0);
        }
        
        private class BuildStepNode
        {
            public int depth;
            public List<BuildStepNode> children;
            public bool foldoutState;
            
            private BuildStep? _step;
            private LogType _worstChildrenLogType;

            public BuildStepNode(BuildStep? step, int _depth)
            {
                _step = step;
                depth = _depth;
                children = new List<BuildStepNode>();

                _worstChildrenLogType = LogType.Log;
                if(_step.HasValue)
                {
                    foreach (var message in _step.Value.messages)
                    {
                        _worstChildrenLogType = message.type; // Warning
                        if (ErrorLogTypes.Contains(message.type))
                            break; // Error
                    }
                }

                foldoutState = false;
            }

            internal void UpdateWorstChildrenLogType()
            {
                foreach(var child in children)
                {
                    child.UpdateWorstChildrenLogType();
                    _worstChildrenLogType = WorseLogType(_worstChildrenLogType, child._worstChildrenLogType);
                }
            }

            public void LayoutGUI(ref bool switchBackgroundColor, float indentPixels)
            {
                switchBackgroundColor = !switchBackgroundColor;
                GUILayout.BeginVertical(switchBackgroundColor ? GUIStyleUtilities.OddStyle : GUIStyleUtilities.EvenStyle);
                GUILayout.BeginHorizontal();                
                GUILayout.Space(10 + indentPixels);

                if (children.Any() || (_step.HasValue && _step.Value.messages.Any()))
                {
                    if (_worstChildrenLogType != LogType.Log)
                    {
                        var icon = "console.warnicon.sml";
                        if (_worstChildrenLogType != LogType.Warning)
                            icon = "console.erroricon.sml";
                        foldoutState = EditorGUILayout.Foldout(foldoutState, EditorGUIUtility.TrTextContentWithIcon(_step.GetValueOrDefault().name, icon), true);
                    }
                    else
                    {
                        foldoutState = EditorGUILayout.Foldout(foldoutState, new GUIContent(_step.GetValueOrDefault().name), true);
                    }
                }
                else
                    GUILayout.Label(_step.GetValueOrDefault().name);

                GUILayout.FlexibleSpace();
                GUILayout.Label(_step.GetValueOrDefault().duration.Hours + ":" + 
                                _step.GetValueOrDefault().duration.Minutes.ToString("D2") + ":" + 
                                _step.GetValueOrDefault().duration.Seconds.ToString("D2") + "." + 
                                _step.GetValueOrDefault().duration.Milliseconds.ToString("D3"));
                GUILayout.EndHorizontal();

                if (foldoutState)
                {
                    if (_step.HasValue)
                    {
                        foreach (var message in _step.Value.messages)
                        {
                            var icon = "console.infoicon.sml";
                            var oldCol = GUI.color;
                            switch (message.type)
                            {
                                case LogType.Warning:
                                    GUI.color = Color.yellow;
                                    icon = "console.warnicon.sml";
                                    break;
                                case LogType.Error:
                                case LogType.Exception:
                                case LogType.Assert:
                                    GUI.color = Color.red;
                                    icon = "console.erroricon.sml";
                                    break;
                            }
                            GUILayout.BeginHorizontal();
                            {
                                GUILayout.Space(20 + indentPixels);
                                GUILayout.Label(EditorGUIUtility.IconContent(icon), GUILayout.ExpandWidth(false));
                                var style = EditorStyles.label;
                                style.wordWrap = true;
                                EditorGUILayout.LabelField(new GUIContent(message.content, message.content), style);
                            }
                            GUILayout.EndHorizontal();
                            GUI.color = oldCol;
                        }
                    }

                    foreach (var child in children)
                        child.LayoutGUI(ref switchBackgroundColor, indentPixels + 20);
                }
                GUILayout.EndVertical();
            }
        }
        
        private static LogType WorseLogType(LogType log1, LogType log2)
        {
            if (ErrorLogTypes.Contains(log1) || ErrorLogTypes.Contains(log2))
                return LogType.Error;
            if (log1 == LogType.Warning || log2 == LogType.Warning)
                return LogType.Warning;
            return LogType.Log;
        }
    }
}