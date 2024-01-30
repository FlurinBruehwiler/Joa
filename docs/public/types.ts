export type apiMethod = {
    signature: string,
    description: string,
    parameters: apiParameter[],
    returns?: apiReturn,
    // exceptions: apiException[]
}

export type apiParameter = {
    name: string
    type: apiType2,
    description: string
}

export type apiType2 = {
    name: string,
    url: string,
    genericParams: apiType2[]
}

export type apiReturn = {
    type: apiType2,
    description: string
}

export type apiType = {
    namespace: string
    name: string,
    genericParameters: apiType[],
    url: string,
    description: string,
    methods: apiMethod[],
    properties: apiProperty[],
    signature: string,
    type: "interface" | "class" | "enum" | "record"
}

export type apiProperty = {
    name: string,
    description: string,
    type: apiType2
    signature: string
}

export {}
