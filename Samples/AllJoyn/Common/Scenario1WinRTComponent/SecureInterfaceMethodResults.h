//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// This code is licensed under the MIT License (MIT).
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************
//-----------------------------------------------------------------------------
// <auto-generated> 
//   This code was generated by a tool. 
// 
//   Changes to this file may cause incorrect behavior and will be lost if  
//   the code is regenerated.
//
//   Tool: AllJoynCodeGenerator.exe
//
//   This tool is located in the Windows 10 SDK and the Windows 10 AllJoyn 
//   Visual Studio Extension in the Visual Studio Gallery.  
//
//   The generated code should be packaged in a Windows 10 C++/CX Runtime  
//   Component which can be consumed in any UWP-supported language using 
//   APIs that are available in Windows.Devices.AllJoyn.
//
//   Using AllJoynCodeGenerator - Invoke the following command with a valid 
//   Introspection XML file and a writable output directory:
//     AllJoynCodeGenerator -i <INPUT XML FILE> -o <OUTPUT DIRECTORY>
// </auto-generated>
//-----------------------------------------------------------------------------
#pragma once

namespace com { namespace microsoft { namespace Samples { namespace SecureInterface {

ref class SecureInterfaceConsumer;

public ref class SecureInterfaceConcatenateResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property Platform::String^ OutStr
    {
        Platform::String^ get() { return m_interfaceMemberOutStr; }
    internal:
        void set(_In_ Platform::String^ value) { m_interfaceMemberOutStr = value; }
    }
    
    static SecureInterfaceConcatenateResult^ CreateSuccessResult(_In_ Platform::String^ interfaceMemberOutStr)
    {
        auto result = ref new SecureInterfaceConcatenateResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->OutStr = interfaceMemberOutStr;
        return result;
    }
    
    static SecureInterfaceConcatenateResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new SecureInterfaceConcatenateResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    Platform::String^ m_interfaceMemberOutStr;
};

public ref class SecureInterfaceJoinSessionResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property SecureInterfaceConsumer^ Consumer
    {
        SecureInterfaceConsumer^ get() { return m_consumer; }
    internal:
        void set(_In_ SecureInterfaceConsumer^ value) { m_consumer = value; }
    };

private:
    int32 m_status;
    SecureInterfaceConsumer^ m_consumer;
};

public ref class SecureInterfaceGetIsUpperCaseEnabledResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    property bool IsUpperCaseEnabled
    {
        bool get() { return m_value; }
    internal:
        void set(_In_ bool value) { m_value = value; }
    }

    static SecureInterfaceGetIsUpperCaseEnabledResult^ CreateSuccessResult(_In_ bool value)
    {
        auto result = ref new SecureInterfaceGetIsUpperCaseEnabledResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        result->IsUpperCaseEnabled = value;
        return result;
    }

    static SecureInterfaceGetIsUpperCaseEnabledResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new SecureInterfaceGetIsUpperCaseEnabledResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
    bool m_value;
};

public ref class SecureInterfaceSetIsUpperCaseEnabledResult sealed
{
public:
    property int32 Status
    {
        int32 get() { return m_status; }
    internal:
        void set(_In_ int32 value) { m_status = value; }
    }

    static SecureInterfaceSetIsUpperCaseEnabledResult^ CreateSuccessResult()
    {
        auto result = ref new SecureInterfaceSetIsUpperCaseEnabledResult();
        result->Status = Windows::Devices::AllJoyn::AllJoynStatus::Ok;
        return result;
    }

    static SecureInterfaceSetIsUpperCaseEnabledResult^ CreateFailureResult(_In_ int32 status)
    {
        auto result = ref new SecureInterfaceSetIsUpperCaseEnabledResult();
        result->Status = status;
        return result;
    }

private:
    int32 m_status;
};

} } } } 
