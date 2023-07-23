export { matchers } from './matchers.js';

export const nodes = [
	() => import('./nodes/0'),
	() => import('./nodes/1'),
	() => import('./nodes/2'),
	() => import('./nodes/3'),
	() => import('./nodes/4'),
	() => import('./nodes/5'),
	() => import('./nodes/6'),
	() => import('./nodes/7'),
	() => import('./nodes/8'),
	() => import('./nodes/9'),
	() => import('./nodes/10'),
	() => import('./nodes/11'),
	() => import('./nodes/12'),
	() => import('./nodes/13'),
	() => import('./nodes/14'),
	() => import('./nodes/15'),
	() => import('./nodes/16')
];

export const server_loads = [];

export const dictionary = {
		"/": [2],
		"/about": [3],
		"/communities": [4],
		"/communities/list": [5],
		"/communities/new": [6],
		"/posts": [7],
		"/posts/new": [8],
		"/settings": [9],
		"/user": [10],
		"/user/list": [11],
		"/user/new": [12],
		"/user/[userid]": [13],
		"/[community]": [14],
		"/[community]/posts": [15],
		"/[community]/posts/[postid]": [16]
	};

export const hooks = {
	handleError: (({ error }) => { console.error(error) }),
};

export { default as root } from '../root.svelte';