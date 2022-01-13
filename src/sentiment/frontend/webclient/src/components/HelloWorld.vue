<template>
  <div>
    <div v-for="(tweet, index) in tweets" :key="index">
      <p>{{ index }} {{ tweet }}</p>
      <blockquote class="twitter-tweet"><a href="https://twitter.com/x/status/{{tweet.id}}"></a></blockquote>
      <ul>
        <li>{{tweet.sentiment}}</li>
        <li>{{tweet.trend}}</li>
      </ul>  
    </div>
  </div>
  <!--
  -->
</template>

<script>
export default {
  data() {
    return {
      tweets: []
    }
  },
  name: 'HelloWorld',
  mounted() {
    let connection = new WebSocket('ws://localhost:3000/');
    connection.onmessage = (event) => {
      // Vue data binding means you don't need any extra work to
      // update your UI. Just set the `time` and Vue will automatically
      // update the `<h2>`.
      // console.log(event.data);
      console.log(event.data);
      console.log(typeof event.data); //string
      console.log(JSON.parse(event.data).data);
      console.log(typeof JSON.parse(event.data).data);
      console.log(JSON.parse(JSON.parse(event.data).data));
      console.log(typeof JSON.parse(JSON.parse(event.data).data));
      this.tweets.push(JSON.parse(JSON.parse(event.data).data));
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
h3 {
  margin: 40px 0 0;
}
ul {
  list-style-type: none;
  padding: 0;
}
li {
  display: inline-block;
  margin: 0 10px;
}
a {
  color: #42b983;
}
</style>
