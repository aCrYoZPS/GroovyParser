namespace GroovyParserBackend.Entities
{
    public enum TokenType
    {
        None,

        NumberLiteral,
        StringLiteral,

        Identifier,
        Keyword,

        FunctionCall, // () )))

        TypeAnnotation,

        Brackets, // {}
        Braces, // []

        Equal, // ==
        GreaterThan,    // [DONE]
        LessThan,       // [DONE]
        GreaterOrEqual, // [DONE]
        LessOrEqual,    // [DONE]
        NotEqual,
        Identical, // === ))))))))
        NotIdentical, // !==))))))

        TernaryOperator,

        ElvisOperator,  // [DONE] sth ?: sth_other
        ElvisAssignment,// [DONE] sth ?= sth_other

        BitwiseAnd,
        BitwiseOr,
        BitwiseNot,
        BitwiseXor,
        LeftShift,      // [DONE] <<
        RightShift,     // [DONE] >>
        UnsignedRightShift, // [DONE] >>>

        And,
        Or,
        Not,

        Assignment,
        Plus,
        Minus,
        Star,
        Slash,
        Percent,
        DoubleStar,

        PlusAssignment,
        MinusAssignment,
        StarAssignment,
        SlashAssignment,
        PercentAssignment,
        DoubleStarAssignment,

        UnaryPlus,
        UnaryMinus,

        PrefixIncrement,
        PrefixDecrement,
        PostfixIncrement,
        PostfixDecrement,

        MemberAccess, // obj
        NullSafeMemberAccess, // obj.?member
        MethodPointer, // obj.&method

        PatternOperator, // ~sth
        FindOperator, // =~ come and save us...
        MatchOperator, // ==~ сука не заебло же их хуярить операторы

        SpreadOperator, // collection*.method

        RangeOperator, // 1..5 or 'a'..'z'... damn...

        SpaceshipOperator, // <=> я клянусь он реально так называется   // [DONE]

        SubscriptOperator, // arr[0] и это не то же самое, что квадратные скобки...
        SafeSubscriptOperator, // arr?[0] java moment   // [DONE]

        MembershipOperator, // sth in collection

        CoercionOperation, // sth as String

        DiamondOperator, // <> as in List<Integer> list = new List<>()
    }
}
