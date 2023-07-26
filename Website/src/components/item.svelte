<!-- This is the posts you see in /posts and  -->

<script lang="ts">
    export let actor_id : string = "John Doe";
    export let title : string = "Funky title";
    export let server : string = "Funky server";
    export let community : string = "Funky community";
    export let image : MediaImage; 

    export let Date : Date;
    export let CommentCount : number = 0;
    export let Original : URL;
    export let Likes : number = 0;
    
    export let Style : string = "Original"; 
    /* 
    Original, Old
    */







</script>

<!-- This needs to have two different ig styles depednent on if a property is true or not-->
was
<!-- svelte-ignore a11y-missing-attribute -->
<img src=image loading="lazy"/>

<style>

</style>

