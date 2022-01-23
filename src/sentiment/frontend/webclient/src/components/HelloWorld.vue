<template>
  <div>
    <div class="content">
      <h2>Tweets with Sentiment</h2>
      <div class="listing" v-for="tweet in tweets" :key="tweet.id">
        <div :class="tweet.sentiment">
          <div class="sentiment">Sentiment: {{tweet.sentiment}}</div>
          <div class="tweet"><Tweet :id="tweet.id" ></Tweet></div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { Tweet } from 'vue-tweet-embed'

export default {
  data() {
    return {
      tweets: []
    }
  },
  name: 'HelloWorld',
  components: {
    Tweet
  },
  mounted() {
    //useful when working locally:
    // setInterval(() => {
    //   this.tweets.unshift({ "id": "1481849540303114244", "sentiment": "positive", "trend": "Sinema", "sentiment_scores": { "positive": 0.275, "neutral": 0.725, "negative": 0, "compound": 0.4648 } });
    // }, 15000);

    let connection = new WebSocket('ws://' + window.location.host + '/');
    connection.onmessage = (event) => {
      console.log(event.data);
      // console.log(event.data);
      // console.log(typeof event.data); //string
      // console.log(JSON.parse(event.data).data);
      // console.log(typeof JSON.parse(event.data).data);
      // console.log(JSON.parse(JSON.parse(event.data).data));
      // console.log(typeof JSON.parse(JSON.parse(event.data).data));
      this.tweets.unshift(JSON.parse(JSON.parse(event.data).data));
    }
  }
}
</script>

<!-- Add "scoped" attribute to limit CSS to this component only -->
<style scoped>
.content {
  margin: 0 auto;
  max-width: 800px;
  padding: 0 20px;
}

.listing {
  margin-bottom: 20px;
}

.sentiment {
  font-size: 0.8em;
  font-weight: bold;
}

.tweet {
  margin-top: 0.5em;
  padding-left: 8em;
}

.positive {
  color: #009900;
  background-color: #e6ffe6;
}

.negative {
  color: #990000;
  background-color: #ffe6e6;
}

.neutral {
  color: #999999;
  background-color: #ffffe0;
}

</style>
