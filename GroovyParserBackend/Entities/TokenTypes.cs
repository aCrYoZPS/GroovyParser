namespace GroovyParserBackend.Entities
{
    public enum TokenType
    {
        NumberLiteral,
        StringLiteral,

        Identifier,
        Keyword,

        FunctionCall, // () )))

        TypeAnnotation,

        Brackets, // {}
        Braces, // []

        Equals,
        DoubleEquals,
        GreaterThan,
        LessThan,
        GreaterOrEqual,
        LessOrEqual,
        NotEqual,
        Identical, // === ))))))))
        NotIdentical, // !==))))))

        TernaryOperator,

        ElvisOperator, // sth ?: sth_other
        ElvisAssignment, // sth ?= sth_other

        BitwiseAnd,
        BitwiseOr,
        BitwiseNot,
        BitwiseXor,
        LeftShift, // <<
        RightShift, // >>
        UnsignedRightShift, // >>>

        And,
        Or,
        Not,

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

        SpaceshipOperator, // <=> я клянусь он реально так называется

        SubscriptOperator, // arr[0] и это не то же самое, что квадратные скобки...
        SafeSubscriptOperator, // arr?[0] java moment

        MembershipOperator, // sth in collection

        CoercionOperation, // sth as String

        DiamondOperator, // <> as in List<Integer> list = new List<>()
    }
}
