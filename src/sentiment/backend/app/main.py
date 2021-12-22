from typing import Optional
from pydantic import BaseModel
from fastapi import BackgroundTasks, FastAPI
from starlette.responses import RedirectResponse
from vaderSentiment.vaderSentiment import SentimentIntensityAnalyzer
import json
import os
import requests

daprPort = os.getenv("DAPR_HTTP_PORT", 3500)

stateStoreName = "statestore"
stateUrl = f"http://localhost:{daprPort}/v1.0/state/{stateStoreName}"
# pubsubName = "pubsub"
# pubSubUrl = f"http://localhost:{daprPort}/v1.0/publish/{pubsubName}/tweets"

app = FastAPI()

@app.get("/")
async def read_root():
    return RedirectResponse(url='/docs')

@app.get("/health")
async def health():
    return {"health": "OK"}

class UserInput(BaseModel):
    text: str
    id: str
    trend: str

class SentimentScores(BaseModel):
    positive: float
    neutral: float
    negative: float
    compound: float

class TweetMeta(BaseModel):
    id: str
    sentiment: str
    trend: str
    sentiment_scores: dict

#TODO: this should write to a real database - or switch to pubsub?
def write_data(data: TweetMeta):
    response = requests.get(f"{stateUrl}/tweet")
    currentValue = response.json()
    if not currentValue:
        currentValue = []
    newValue = currentValue + [json.dumps(data.__dict__)]
    state = [{
        "key": "tweet",
        "value": newValue
    }]
    requests.post(stateUrl, json=state)

@app.post("/evaluate")
async def evaluate(user_input: UserInput, background_tasks: BackgroundTasks):
    analyzer = SentimentIntensityAnalyzer()
    scores = analyzer.polarity_scores(user_input.text)

    sentiment = "neutral"
    if scores['compound'] >= 0.05:
        sentiment = 'positive'
    elif scores['compound'] <= -0.05:
        sentiment = 'negative'

    sentiment_scores = SentimentScores(
        positive = scores['pos'],
        neutral = scores['neu'],
        negative = scores['neg'],
        compound = scores['compound']
    )

    data = TweetMeta(id=user_input.id, sentiment=sentiment, trend=user_input.trend, sentiment_scores=sentiment_scores.__dict__)

    background_tasks.add_task(write_data, data)

    return data

@app.get("/tweets")
async def get_tweets():
    response = requests.get(f"{stateUrl}/tweet")
    return response.json()