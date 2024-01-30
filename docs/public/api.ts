import fs from "fs";
import YAML from "yaml";
import type {apiMethod, apiProperty, apiType, apiType2} from "./types";
import type {ymlRoot} from "./ymlType";
import {SUB_URL} from "../src/consts";

const types: apiType[] = [];
const files = fs.readdirSync("./api");

const typesNames: string[] = []

const yamls: ymlRoot[] = [];

files.forEach(p => {
    const file = fs.readFileSync("./api/" + p, 'utf8');
    const yaml: ymlRoot = YAML.parse(file)

    if (!yaml.items)
        return;

    yamls.push(yaml)

    yaml.items.forEach(i => {
        if (i.type === "Interface" || i.type === "Class" || i.type === "Enum" || i.type === "Record") {
            typesNames.push(i.fullName)
        }
    })
})

const getApiType = (type: string): apiType2  => {
    const generics = extractGenerics(type)
    const typeName = type.split("{")[0]

    return {
        name: typeName,
        url: getUrl(typeName),
        genericParams: generics.map(x => getApiType(x))
    }
}


function extractGenerics(input: string): string[] {
    let result: string[] = [];

    let stack: string[] = [];
    let startIndex = -1;

    for (let i = 0; i < input.length; i++) {
        const char = input[i];

        if (char === "{") {
            if (stack.length === 0) {
                startIndex = i + 1;
            }
            stack.push("{");
        } else if (char === "}") {
            stack.pop();
            if (stack.length === 0 && startIndex !== -1) {
                result.push(input.substring(startIndex, i));
                startIndex = -1;
            }
        }
    }

    return result;
}

const getUrl = (type: string | undefined)  => {
    if(!type){
        return "void"
    }
    if(typesNames.find(x => x == type)){
        return SUB_URL + "/types/" + type.split(".").slice(-1)
    }
    return "https://learn.microsoft.com/en-us/dotnet/api/" + type.toLowerCase();
}

yamls.forEach(yaml => {
    let type: apiType | undefined;

    const getType = (t: string): "interface" | "class" | "enum" | "record" => {
        if (t === "Interface")
            return "interface"
        if (t === "Class")
            return "class"
        if (t === "Enum")
            return "enum"
        if (t === "Record")
            return "record"
        throw "Unknown type: " + t;
    }

    if (!yaml.items)
        return;

    yaml.items.forEach(i => {
        if (i.type === "Interface" || i.type === "Class" || i.type === "Enum" || i.type === "Record") {
            type = {
                type: getType(i.type),
                name: i.name,
                description: i.summary ?? "",
                url: SUB_URL.replace("/", "") + "/types/" + i.name,
                namespace: i.parent,
                methods: [],
                genericParameters: [],
                properties: [],
                signature: i.syntax.content
            }
        }
        if (i.type === "Property" && type) {
            if(!i.syntax.return)
                throw "Property must have return type"
            let property: apiProperty = {
                type: getApiType(i.syntax.return.type),
                signature: i.syntax.content,
                description: i.summary ?? "",
                name: i.name
            }
            type.properties.push(property)
        }
        if (i.type === "Method" && type) {
            let method: apiMethod = {
                signature: i.syntax.content,
                description: i.summary ?? "",
                returns: i.syntax.return ? {
                    type: getApiType(i.syntax.return.type),
                    description: i.syntax.return.description ?? ""
                } : undefined,
                parameters: i.syntax.parameters?.map(x => {
                    return {
                        name: x.id,
                        description: x.description,
                        type: getApiType(x.type)
                    }
                }) ?? [],
            }
            type.methods.push(method)
        }
    })

    if (type)
        types.push(type)
})

const namespaces: { name: string, types: apiType[] }[] = []

types.forEach(type => {
    const existingNamespace = namespaces.find(x => x.name === type.namespace);
    if(existingNamespace){
        existingNamespace.types.push(type)
        return;
    }
    namespaces.push({name: type.namespace, types: [type]})
})

export {types, namespaces, getUrl}
