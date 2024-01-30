export interface ymlRoot {
    items: ymlItem[]
    references: ymlReference[]
}

export interface ymlItem {
    uid: string
    commentId: string
    id: string
    parent: string
    children?: string[]
    langs: string[]
    name: string
    nameWithType: string
    fullName: string
    type: string
    source: ymlSource
    assemblies: string[]
    namespace: string
    syntax: ymlSyntax
    summary?: string
    example?: any[]
    overload?: string
    "nameWithType.vb"?: string
    "fullName.vb"?: string
    "name.vb"?: string
}

export interface ymlSource {
    remote: ymlRemote
    id: string
    path: string
    startLine: number
}

export interface ymlRemote {
    path: string
    branch: string
    repo: string
}

export interface ymlSyntax {
    content: string
    "content.vb": string
    parameters?: ymlParameter[]
    return?: ymlReturn
}

export interface ymlParameter {
    id: string
    type: string
    description: string
}

export interface ymlReturn {
    type: string
    description: string
}

export interface ymlReference {
    uid: string
    commentId: string
    name: string
    nameWithType: string
    fullName: string
    "spec.csharp"?: ymlSpecCsharp[]
    "spec.vb"?: ymlSpecVb[]
    parent?: string
    isExternal?: boolean
    "nameWithType.vb"?: string
    "fullName.vb"?: string
    "name.vb"?: string
    definition?: string
}

export interface ymlSpecCsharp {
    uid?: string
    name: string
    isExternal?: boolean
}

export interface ymlSpecVb {
    uid?: string
    name: string
    isExternal?: boolean
}

export {}
