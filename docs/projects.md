# 项目列表

本页列举整个仓库使用到的项目，以及它的用途和功能。

## 类库项目

类库项目指的是它们自身不包含任何主入口点来直接运行起来的项目。类库只提供 API 的封装，包裹众多 API，将其打包成一个项目的项目类型。

属于该类型的项目有：

* [`Sudoku.Compatibility`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Compatibility)：提供一些跟其他程序交互的兼容性处理 API，诸如 Hodoku 软件的难度系数对应技巧的分数的信息、Sudoku Explainer 软件对技巧提供的名字信息等；
* [`Sudoku.Core`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Core)：提供基本的数独相关的数据结构的实现，如数独盘面的实现 [`Grid`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Core/Collections/Grid.cs) 类型等；
* [`Sudoku.Gdip`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Gdip)：提供一种简单、轻量级的数独绘图 API；
* [`Sudoku.IO`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.IO)：提供 I/O 操作的 API，例如交互题库文件；
* [`Sudoku.Presentation`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Presentation)：提供 API 用于专门呈现数独或数独有关的一些东西用在 UI 上，比如一个步骤（即 `IStep` 类型）的 UI 呈现视图的抽象数据结构；
* [`Sudoku.Platforms.QQ`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Platforms.QQ)：提供 API 专门用于操作和建立 Mirai 连接，用于 QQ 平台，创建机器人模式的自动化处理的相关功能（比如数独的绘图、数独的 PK 竞技等）；
* [`Sudoku.Recognition`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Recognition)：提供识别一个图片里的数独题目的基本 API（实现得非常简单）；
* [`Sudoku.Solving.Algorithms`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Algorithms)：提供对于暴力破解的数独解题算法的 API，比如位运算、回溯递归算法等；
* [`Sudoku.Solving.Logical`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Logical)：提供对于逻辑解题过程处理的 API；
* [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System)：为整个解决方案的别的项目提供关于 .NET 基本库 API 拓展 API 或功能代码。

## 源代码生成器项目

**源代码生成器**（Source Generator）也经常被简称为源生成器或者源码生成器，不过源代码生成器这个名字更贴近它的真正用法和作用对象。正常在看这个术语的时候，如果没有写清楚是源代码的话，读者有可能会不知道源生成器的“源”是什么。

属于该类型的项目有：

* [`Global.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Global.CodeGen)：为高阶的源代码生成器。它生成一些东西，专门提供给其它的源代码生成器项目使用，也提供一些 API；
* [`Sudoku.Diagnostics.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeGen)：为解决方案提供一些基本的、不必手写的源代码的功能性扩展；
* [`Sudoku.Diagnostics.CodeAnalysis.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeAnalysis.CodeGen)：提供对  [`Sudoku.Diagnostics.CodeAnalysis`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Diagnostics.CodeAnalysis/Sudoku.Diagnostics.CodeAnalysis) 项目进行代码生成服务；
* [`Sudoku.Solving.Logical.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Logical.CodeGen)：提供对 [`Sudoku.Solving.Logical`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.Solving.Logical) 项目的源代码生成服务；
* [`System.CodeGen`](https://github.com/SunnieShine/Sudoku/tree/main/src/System.CodeGen)：提供对 [`System`](https://github.com/SunnieShine/Sudoku/tree/main/src/System) 项目的源代码生成服务。

## 解决方案项目

这些项目将对底层的 API 进行调用，实现对于控制台、UI 界面框架等的用户交互解决方案。

属于该类型的项目有：

* [`Sudoku.CommandLine`](https://github.com/SunnieShine/Sudoku/tree/main/src/Sudoku.CommandLine)（控制台）：提供一个关于数独基本运算和操作的控制台实现。
* [`SudokuStudio`](https://github.com/SunnieShine/Sudoku/tree/main/src/SudokuStudio)（UI）：提供一个关于数独基本运算和操作的 UI 级别实现。

