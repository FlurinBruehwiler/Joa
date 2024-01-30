export const SITE = {
    title: 'Joa',
    description: 'Your website description.',
    defaultLanguage: 'en-us',
} as const;

export const OPEN_GRAPH = {
    image: {
        src: 'https://github.com/withastro/astro/blob/main/assets/social/banner-minimal.png?raw=true',
        alt:
            'astro logo on a starry expanse of space,' +
            ' with a purple saturn-like planet floating in the right foreground',
    },
    twitter: 'astrodotbuild',
};

export const KNOWN_LANGUAGES = {
    English: 'en',
} as const;
export const KNOWN_LANGUAGE_CODES = Object.values(KNOWN_LANGUAGES);

export const GITHUB_EDIT_URL = `https://github.com/Joa-Launcher/Api-Documentation/blob/develop/docs`;
export const GITHUB_CODE_URL = `https://github.com/Joa-Launcher/Plugin-Api/blob/develop`;

export const SUB_URL = "/Api-Documentation"

export type Sidebar = Record<
    (typeof KNOWN_LANGUAGE_CODES)[number],
    Record<string, { text: string; link: string }[]>
>;
export const SIDEBAR: Sidebar = {
    en: {
        'Getting Started': [
            {text: 'Introduction', link: SUB_URL + '/en/introduction'},
			{text: 'Overview', link: SUB_URL + '/en/overview'},
			{text: 'Settings', link: SUB_URL + '/en/settings'},
		        {text: 'Index', link: SUB_URL + '/en/index'},
			{text: 'Dependency Injection', link: SUB_URL + '/en/dependency_injection'}
        ]
	},
};
