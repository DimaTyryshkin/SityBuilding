using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UIElements;

namespace CellularAutomaton
{
    public class CellRulesEditor :  EditorWindow
    {
        static readonly string assetPath = "ObjectsPalette";
        static readonly string bsdKey = "CellularAutomaton.CellRulesEditor.Save";
        Rules rules;
        
        [OnOpenAsset(1)]
        public static bool OnOpenAsset(int instanceID, int line)
        {
            EditorUtility.InstanceIDToObject(instanceID);
            if (EditorUtility.InstanceIDToObject(instanceID) is Rules rules)
            {
                var winow = GetWindow<CellRulesEditor>(title: rules.name);
                winow.rules = rules;
                winow.Show();
                return true;
            }
            else
            {
                return false;
            }
        }

        GUIStyle paletteToggle;
        GUIStyle rulesToggle;
        
        void Validate()
        {
            if(!rules)
                return;
                
            // Styles
            paletteToggle = new GUIStyle(EditorStyles.miniButton);
            rulesToggle = new GUIStyle(EditorStyles.miniButton);
            //paletteToggle.normal.textColor = Color.grey;
            //paletteToggle.active.textColor = Color.grey; 
            
            // Pallete
            maskAndColors = new List<MaskAndColor>();
            
            maskAndColors.Add(new MaskAndColor()
            {
                mask = CellMask.None, 
                color = Color.white,
            });
            
            foreach (var typeAndColor in rules.colors)
            {
                maskAndColors.Add(new MaskAndColor()
                {
                    mask = (CellMask)typeAndColor.type,
                    color =  typeAndColor.color,
                });
            }
            
            //  
        }

        public void SetRiles(Rules rules)
        {
            this.rules = rules;
        }


        List<MaskAndColor> maskAndColors; 
        CellMask brush;
        Vector2 scrollPos;
        float tab = 30;
        int btnWidth = 30; 
        float matrixSpace = 10;
        float matrixLineSpace = 20;
        void OnGUI()
        { 
            Validate();

            if (!rules)
            {
                GUILayout.Label("Нет объекта");
                return; 
            }
            
            GUILayout.BeginVertical();
            {
                // Палитра
                GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                {
                    foreach (var maskAndColor in maskAndColors)
                    {
                        bool isToggle = brush == maskAndColor.mask;
                        string content = isToggle ? "✔" : "";
                        GUI.backgroundColor = maskAndColor.color;
                        if (GUILayout.Toggle(isToggle, content, paletteToggle, GUILayout.Width(40)))
                            brush = maskAndColor.mask;
                    }
                }
                GUILayout.EndHorizontal();

                GUI.backgroundColor = Color.white;
                
                scrollPos = GUILayout.BeginScrollView(scrollPos,GUILayout.ExpandWidth(false));
                {
                    GUILayout.Label("Группы правил");
                    for (int i = 0; i < rules.groups.Count; i++)
                    {
                        RulesGroup group = rules.groups[i];
                        GUI.contentColor = rules.CellTypeToColor(group.cellType);
                        group.isOpen = EditorGUILayout.Foldout(group.isOpen, group.cellType.ToString());
                        if (group.isOpen )
                            DrawGroup(group);
                        
                        GUI.contentColor = Color.white;

                    }
                }
                GUILayout.EndScrollView();
            }
            GUILayout.EndVertical();
            
            
            GUILayout.BeginVertical();
            {
                
            }
            GUILayout.EndVertical(); 
            
            GUILayout.BeginHorizontal();
            {
                
            }
            GUILayout.EndHorizontal();
        }

        void DrawGroup(RulesGroup group)
        {
            GUILayout.BeginVertical();
            { 
                for (int i = 0; i < group.rules.Count; i++)
                {
                    GUILayout.BeginHorizontal(GUILayout.ExpandWidth(false));
                    { 
                        GUILayout.Space(tab);
                        DrawRule(group.rules[i]); 
                        // Edit list
                        GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
                        {
                            if (GUILayout.Button("✖", GUILayout.Width(btnWidth)))
                                group.rules.RemoveAt(i);

                            if (GUILayout.Button("▲", GUILayout.Width(btnWidth)))
                                TryReplace(group.rules, i, i - 1);

                            if (GUILayout.Button("▼", GUILayout.Width(btnWidth)))
                                TryReplace(group.rules, i, i + 1);

                            if (GUILayout.Button("✚", GUILayout.Width(btnWidth)))
                            {
                                group.rules.Insert(i + 1, Rule.Create());
                                EditorUtility.SetDirty(rules);
                            }
                        }
                        GUILayout.EndVertical(); 
                    }
                    GUILayout.EndHorizontal();
                    
                    if (i < group.rules.Count - 1)
                        GUILayout.Space(matrixLineSpace);
                }

                if (group.rules.Count == 0) 
                    if (GUILayout.Button("✚", GUILayout.Width(btnWidth)))
                        group.rules.Add(Rule.Create());
            }
            GUILayout.EndVertical(); 
        }

        void TryReplace(List<Rule> rules, int index1, int index2)
        {
            if (index1 < 0 || index1 >= rules.Count) return;
            if (index2 < 0 || index2 >= rules.Count) return;

            Rule temp = rules[index1];
            rules[index1] = rules[index2];
            rules[index2] = temp;
        }

        void DrawRule(Rule rule)
        {

            Rect r1 = GUILayoutUtility.GetRect(90, 90, GUILayout.ExpandWidth(false));
            GUILayout.Space(matrixSpace);
            Rect r2 = GUILayoutUtility.GetRect(90, 90, GUILayout.ExpandWidth(false));

            DrawMask(r1, rule.mainVariant.mask);
            DrawMask(r2, rule.mainVariant.result);

            GUILayout.BeginVertical(GUILayout.ExpandWidth(false));
            {
                rule.rotation = GUILayout.Toggle(rule.rotation, "↻",rulesToggle, GUILayout.Width(btnWidth), GUILayout.ExpandWidth(false));
                rule.repeat = GUILayout.Toggle(rule.repeat, "∞", rulesToggle, GUILayout.Width(btnWidth), GUILayout.ExpandWidth(false));
            }
            GUILayout.EndVertical();
        }

        void DrawMask(Rect rect, CellMask[] mask)
        {
            float step = rect.height / Rule.n;
            Vector2 pos = rect.position;

            for (int x = 0; x < Rule.n; x++)
            for (int y = 0; y < Rule.n; y++)
            {
                int index = RuleVariant.GetIndex(x, y);
                Rect buttonRect = new Rect(pos.x + x * step, pos.y + y * step, step, step);
                GUI.backgroundColor = GetColor(mask[index]);
                if (GUI.Button(buttonRect, ""))
                    mask[index] = brush;
            }
            
            GUI.backgroundColor= Color.white;
        } 

        Color GetColor(CellMask mask)
        {
            for (int i = 0; i < maskAndColors.Count; i++)
            {
                if (maskAndColors[i].mask == mask)
                    return maskAndColors[i].color;
            }

            throw new Exception("Color for mask not found");
            return Color.magenta;
        }

        struct MaskAndColor
        {
            public CellMask mask;
            public Color color;
        }
    }
}
