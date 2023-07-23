<nav>
    <div class="navbar bg-slate-200 dark:bg-zinc-800 ">
        <a class="" href="/">            
            <img src="/favicon.ico" alt="white hashtag on a black background" /> 
            DotFed </a>
        <a class="btn" href="/about"> 
            ? 
        </a>        
        <button class="btn btn-secondary" on:click={ flip }> {darkmode == true ? 'lightmode' : 'darkmode'} </button>
    </div>
</nav>


<script lang="ts">

    let darkmode : boolean = true
    function flip(){
        darkmode = !darkmode
    }

</script>

<style>
    

</style>