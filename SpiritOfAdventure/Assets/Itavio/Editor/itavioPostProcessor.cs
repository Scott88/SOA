/*
 * Copyright (c) 2015. KinderGuardian Inc.
 */

using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif

using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;

public class itavioPostProcessor : MonoBehaviour {

    #if UNITY_IOS
    // XCode Project Files
    private static string XCodeProjPBX_Ext = ".xcodeproj/project.pbxproj";
    private static string InfoPlist = "Info.plist";

    // Framework Path
    private static string ITAVIO_FRAMEWORK_PATH = "Frameworks/Plugins/iOS/ItavioSdk.framework";

    // PList Keys
    private static string CFBundleIdentifier = "CFBundleIdentifier";
    private static string CFBundleURLTypes = "CFBundleURLTypes";
    private static string CFBundleURLName = "CFBundleURLName";
    private static string CFBundleURLSchemes = "CFBundleURLSchemes";

    // PList Strings
    private static string PRODUCT_NAME = "${PRODUCT_NAME}";
    private static string URL_BASE = "com.itavio.parentapp.";
    private static string URL_PRODUCT_NAME = "$(PRODUCT_NAME:rfc1034identifier)";

    // PBX Properties
    private static string RUNPATH_SEARCH_PATHS = "LD_RUNPATH_SEARCH_PATHS";
    private static string EXECUTABLE_PATH_FRAMEWORKS = "@executable_path/Frameworks";

    // Manual PBX Sections
    private static string PBXBuildFile_Section = "PBXBuildFile";
    private static string PBXCopyFilesBuildPhase_Section = "PBXCopyFilesBuildPhase";
    private static string PBXNativeTarget_Section = "PBXNativeTarget";

    // Manual PBX Section Lines
    private static string ItavioSdk_In_CopyFiles_Line = "ItavioSdk.framework in CopyFiles";
    private static string BuildPhases_Line = "buildPhases = (";
    private static string BuildPhases_End_Line = ");";
    private static string Resources_Line = "Resources";
    private static string ShellScript_Line = "ShellScript";
    private static string CopyFiles_Line = "CopyFiles";
    private static string Sources_Line = "Sources";
    private static string Frameworks_Line = "Frameworks";


    // Manual PBX Section Format Strings
    private static string PBX_BEGIN_SECTION = "/* Begin {0} section */";
    private static string PBX_END_SECTION = "/* End {0} section */";

    // Manual PBX PBXBuildFile CopyFile Format String
    private static string PBX_BuildFile_CopyFiles ="{0} /* ItavioSdk.framework in CopyFiles */ = {{isa = PBXBuildFile; fileRef = {1} /* ItavioSdk.framework */; settings = {{ATTRIBUTES = (CodeSignOnCopy, RemoveHeadersOnCopy, ); }}; }};";
    private static string PBX_CopyFilesBuildPhase_CopyFiles = "{0} /* CopyFiles */ = {{isa = PBXCopyFilesBuildPhase; buildActionMask = 2147483647; dstPath = \"\"; dstSubfolderSpec = 10; files = ( {1} /* ItavioSdk.framework in CopyFiles */, ); runOnlyForDeploymentPostprocessing = 0; }};";
    private static string PBX_NativeTarget_BuildPhase = "{0} /* {1} */,";

    // Lines in Project PBX for Manual Editing
    private static List<string> s_pbxLines;
    #endif

    [PostProcessBuild]
    public  static  void OnPostprocessBuild (BuildTarget BuildTarget, string path)
    {
        #if UNITY_IOS
        if (BuildTarget == UnityEditor.BuildTarget.iOS)
        {
            OnPostprocessBuild_UpdateProjectPBX(path);
            OnPostprocessBuild_UpdatePlist(path);
        }
        #endif
    }

    #if UNITY_IOS
    private static void OnPostprocessBuild_UpdateProjectPBX(string path)
    {
        string projPath = Path.Combine(path, PBXProject.GetUnityTargetName() + XCodeProjPBX_Ext);
        PBXProject proj = new PBXProject();

        proj.ReadFromString(File.ReadAllText(projPath));
        string target = proj.TargetGuidByName(PBXProject.GetUnityTargetName());

        proj.AddBuildProperty(target, RUNPATH_SEARCH_PATHS, EXECUTABLE_PATH_FRAMEWORKS);

        proj.WriteToFile(projPath);

        // CopyFiles BuildPhase
        OnPostProcessBuild_ManualCopyBuildPhase(projPath, proj.FindFileGuidByProjectPath(ITAVIO_FRAMEWORK_PATH), PBXProject.GetUnityTargetName());
    }


    private static void OnPostProcessBuild_ManualCopyBuildPhase(string projPath, string itavioSdkGUID, string target)
    {
        s_pbxLines = new List<string>(File.ReadAllLines(projPath));
        if (FindLineInSection(ItavioSdk_In_CopyFiles_Line, PBXBuildFile_Section) < 0 && 
            FindLineInSection(ItavioSdk_In_CopyFiles_Line, PBXCopyFilesBuildPhase_Section) < 0)
        {
            string copyFileGuid = NewGuid();
            string copyFilePhaseGuid = NewGuid();

            InsertLineIntoSection(string.Format(PBX_BuildFile_CopyFiles, copyFileGuid, itavioSdkGUID), PBXBuildFile_Section);
            InsertLineIntoSection(string.Format(PBX_CopyFilesBuildPhase_CopyFiles, copyFilePhaseGuid, copyFileGuid), PBXCopyFilesBuildPhase_Section);
            InsertCopyFilesPhaseToTarget(string.Format(PBX_NativeTarget_BuildPhase, copyFilePhaseGuid, CopyFiles_Line), target);

            File.WriteAllLines(projPath, s_pbxLines.ToArray());
        }
    }

    private static string NewGuid()
    {
        return Guid.NewGuid().ToString("N").Substring(8).ToUpper();
    }

    private static void InsertCopyFilesPhaseToTarget(string copyFilesPhase, string target)
    {
        int sectionStart = FindLineContaining(string.Format(PBX_BEGIN_SECTION, PBXNativeTarget_Section));
        if (sectionStart < 0)
        {
            return;
        }
        
        int targetStart = FindLineContaining(target, sectionStart);
        if (targetStart < 0)
        {
            return;
        }

        int buildphaseStart = FindLineContaining(BuildPhases_Line, targetStart);
        if (buildphaseStart < 0)
        {
            return;
        }
        int buildphaseEnd = FindLineContaining(BuildPhases_End_Line, buildphaseStart);
        int buildphaseCount = buildphaseEnd - buildphaseStart;

        int copyFilesIndex = FindLineContaining(CopyFiles_Line, buildphaseStart, buildphaseCount);
        if (copyFilesIndex < 0)
        {
            int resourcesStart = FindLineContaining(Resources_Line, buildphaseStart, buildphaseCount);
            int shellscriptStart = FindLineContaining(ShellScript_Line, buildphaseStart, buildphaseCount);

            int topValue = Mathf.Max(resourcesStart, shellscriptStart);

            int sourcesStart = FindLineContaining(Sources_Line, buildphaseStart, buildphaseCount);
            int frameworksStart = FindLineContaining(Frameworks_Line, buildphaseStart, buildphaseCount);

            int bottomValue = Mathf.Min(sourcesStart, frameworksStart);


            if (topValue >= 0 )
            {
                s_pbxLines.Insert(shellscriptStart + 1, copyFilesPhase);
            }
            else if (bottomValue >= 0)
            {
                s_pbxLines.Insert(sourcesStart, copyFilesPhase);

            }
        }
        else
        {
            s_pbxLines.Insert(copyFilesIndex + 1, copyFilesPhase);
        }
    }

    private static void InsertLineIntoSection(string line, string section)
    {
        int sectionStart = FindLineContaining(string.Format(PBX_BEGIN_SECTION, section));
        s_pbxLines.Insert(sectionStart + 1, line);
    }

    private static int FindLineInSection(string search, string section)
    {
        int start = FindLineContaining(string.Format(PBX_BEGIN_SECTION, section));
        int end = FindLineContaining(string.Format(PBX_END_SECTION, section), start);
        return FindLineContaining(search, start, end);
    }

    private static int FindLineContaining(string line, int index = 0, int count = -1)
    {
        count = count < 0 ? s_pbxLines.Count : count;
        for (int i = index; i < Mathf.Min(index + count, s_pbxLines.Count); ++i)
        {
            if (s_pbxLines[i].Contains(line))
            {
                return i;
            }
        }
        return -1;
    }

    private static void OnPostprocessBuild_UpdatePlist(string path)
    {
        string plistPath = Path.Combine(path, InfoPlist);
        PlistDocument plist = new PlistDocument();
        plist.ReadFromFile(plistPath);

        if (plist.root.values.ContainsKey(CFBundleIdentifier))
        {
            string bundleIdentifier = plist.root.values[CFBundleIdentifier].AsString(); // com.bundleid.${PRODUCT_NAME}
            bundleIdentifier = bundleIdentifier.Replace(PRODUCT_NAME, string.Empty); // com.bundleid.

            string url = URL_BASE + bundleIdentifier + URL_PRODUCT_NAME;

            bool AddURLType = true;
            PlistElementArray URLTypesArray;
            if (plist.root.values.ContainsKey(CFBundleURLTypes))
            {
                URLTypesArray = plist.root.values[CFBundleURLTypes].AsArray();

                foreach (PlistElement urlItem in URLTypesArray.values)
                {
                    if (urlItem.AsDict().values.ContainsKey(CFBundleURLName) &&
                        urlItem.AsDict().values[CFBundleURLName].AsString() == url)
                    {
                        AddURLType = false;
                    }
                }
            }
            else
            {
                URLTypesArray = plist.root.CreateArray(CFBundleURLTypes);
            }

            if (AddURLType)
            {
                PlistElementDict URLTypesDict = URLTypesArray.AddDict();
                URLTypesDict.SetString(CFBundleURLName, url);
                PlistElementArray URLSchemesArray = URLTypesDict.CreateArray(CFBundleURLSchemes);
                URLSchemesArray.AddString(url);

                plist.WriteToFile(plistPath);
            }
        }
    }
    #endif
}