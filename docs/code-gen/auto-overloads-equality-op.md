# `AutoOverloadsEqualityOperatorsGenerator` 源代码生成器

## 基本介绍

**功能**：通过特性生成 `==` 和 `!=` 运算符的重载规则。

**类型名**：[`AutoOverloadsEqualityOperatorsGenerator`](https://github.com/SunnieShine/Sudoku/blob/main/src/Sudoku.Diagnostics.CodeGen/Generators/AutoOverloadsEqualityOperatorsGenerator.cs)

**所属项目**：`Sudoku.Diagnostics.CodeGen`

**实现接口**：[`ISourceGenerator`](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.isourcegenerator)

**生成的源代码的编程语言**：C#

## 用法

### 基本用法

在早期我们可以通过重载 `==` 以及 `!=` 来完成相等性的比较关系。为了简化使用，我们只需要保证类型实现 `IEquatable<>` 接口的 `Equals` 方法，就可以通过 `AutoOverloadsEqualityOperatorsAttribute` 特性的标记来完成自动代码的生成。

只需要标记该特性到类型声明之上即可。

```csharp
[AutoOverloadsComparisonOperators]
readonly partial struct Utf8Char : IEquatable<Utf8Char>
{
}
```

### `EmitsInKeyword` 属性

该特性上包含一个叫 `EmitsInKeyword` 的可选属性。该属性表示，在实现比较运算符重载的代码的时候，是否给参数上标记 `in` 修饰符。这是为了在实现过程之中能够有更好的性能，毕竟有些时候，结构类型实现起来会比较大，拷贝起来会比较慢，使用 `in` 来表达类型传引用，这样就可以减少拷贝时间，提升一定的性能。

### `WithNullableAnnotation` 属性

该特性还包含一个叫 `WithNullableAnnotation` 的可选属性。该属性表达的情况是，是否给实现的运算符重载的参数上都标记上可空类型记号 `?`。在相等性比较的时候，带有可空类型标记是非常有用的。

## 注意事项

实际上，源代码生成器没有必要验证是否类型实现了 `IEquatable<>` 接口。因为用户为了提升性能，`Equals` 方法的参数也可能会带有 `in` 修饰符，在验证上会有一定的复杂性。因此，只要你的类型包含一个可以 `Equals` 方法调用的方法就可以了，不管参数是否有 `in` 修饰符，也不管你是不是真的实现了 `IEquatable<>` 接口都是不影响代码生成的。