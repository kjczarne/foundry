module Model

type Question' = QuestionM of string
type Answer' = AnswerM of string
type Title' = TitleM of string
type Begin' = BeginM of string
type End' = EndM of string
type Question'' = Question of string
type Answer'' = Answer of string
type Title'' = Title of string
type Begin'' = Begin of string
type End'' = End of string

type Expr =
    | QM of Question'
    | AM of Answer'
    | TM of Title'
    | BM of Begin'
    | EM of End'
    | Q of Question''
    | A of Answer''
    | T of Title''
    | B of Begin''
    | E of End''
    | Error of string
    | N